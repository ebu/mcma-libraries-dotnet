using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Mcma.Data;
using Mcma.Data.DocumentDatabase.Queries;
using Mcma.Serialization;
using Newtonsoft.Json.Linq;

namespace Mcma.Aws.DynamoDb
{
    public class DynamoDbTable : IDocumentDatabaseTable
    {
        public DynamoDbTable(ICustomQueryBuilderRegistry<QueryOperationConfig> customQueryBuilderRegistry,
                             IAttributeMapper attributeMapper,
                             IFilterExpressionBuilder filterExpressionBuilder,
                             IAmazonDynamoDB dynamoDb,
                             DynamoDbTableDescription tableDescription,
                             DynamoDbTableProviderOptions options)
        {
            CustomQueryBuilderRegistry = customQueryBuilderRegistry ?? throw new ArgumentNullException(nameof(customQueryBuilderRegistry));
            AttributeMapper = attributeMapper ?? throw new ArgumentNullException(nameof(attributeMapper));
            FilterExpressionBuilder = filterExpressionBuilder ?? throw new ArgumentNullException(nameof(filterExpressionBuilder));
            TableDescription = tableDescription ?? throw new ArgumentNullException(nameof(tableDescription));
            Options = options ?? new DynamoDbTableProviderOptions();
            Table = dynamoDb != null ? Table.LoadTable(dynamoDb, tableDescription.TableName) : throw new ArgumentNullException(nameof(dynamoDb));
        }

        private ICustomQueryBuilderRegistry<QueryOperationConfig> CustomQueryBuilderRegistry { get; }

        private IAttributeMapper AttributeMapper { get; }

        private IFilterExpressionBuilder FilterExpressionBuilder { get; }

        private DynamoDbTableDescription TableDescription { get; }

        private DynamoDbTableProviderOptions Options { get; }

        private Table Table { get; }

        private static (string partitionKey, string sortKey) ParsePartitionAndSortKeys(string id)
        {
            var lastSlashIndex = id.LastIndexOf('/');
            return lastSlashIndex > 0
                       ? (id.Substring(0, lastSlashIndex), id.Substring(lastSlashIndex + 1))
                       : (id, id);
        }

        private static T DocumentToResource<T>(Document document) where T : class
        {
            var docJson = JObject.Parse(document.ToJson());

            var resourceJson = docJson["resource"];

            return resourceJson?.ToMcmaObject<T>();
        }

        private Document ResourceToDocument<T>(string partitionKey, string sortKey, T resource)
        {
            var resourceJson = resource.ToMcmaJson().RemoveEmptyStrings();
            
            var item = new JObject
            {
                [TableDescription.KeyNames.Partition] = partitionKey,
                [TableDescription.KeyNames.Sort] = sortKey,
                [nameof(resource)] = resourceJson
            };

            foreach (var kvp in AttributeMapper.GetMappedAttributes(partitionKey, sortKey, resource))
                item[kvp.Key] = kvp.Value != null ? JToken.FromObject(kvp.Value) : JValue.CreateNull();

            return Document.FromJson(item.ToString());
        }

        public async Task<QueryResults<T>> QueryAsync<T>(Query<T> query) where T : class
        {
            var keyExpression =
                new Expression
                {
                    ExpressionStatement = "#partitionKey = :partitionKey",
                    ExpressionAttributeNames = new Dictionary<string, string> {["#partitionKey"] = TableDescription.KeyNames.Partition},
                    ExpressionAttributeValues = new Dictionary<string, DynamoDBEntry> {[":partitionKey"] = query.Path}
                };

            var filterExpression = query.FilterExpression != null ? FilterExpressionBuilder.Build(query.FilterExpression) : null;

            var indexName = default(string);
            if (query.SortBy != null)
            {
                var matchingIndex =
                    TableDescription.LocalSecondaryIndexes.FirstOrDefault(
                        lsi => lsi.SortKeyName.Equals(query.SortBy, StringComparison.OrdinalIgnoreCase));
                
                indexName = matchingIndex.Name ?? throw new McmaException($"No matching local secondary index found for sorting by '{query.SortBy}'");
            }
            
            var queryOpConfig = new QueryOperationConfig
            {
                KeyExpression = keyExpression,
                FilterExpression = filterExpression,
                IndexName = indexName,
                BackwardSearch = !query.SortAscending,
                ConsistentRead = Options.ConsistentQuery ?? false,
                Limit = query.PageSize ?? int.MaxValue,
                PaginationToken = query.PageStartToken
            };

            var tableQuery = Table.Query(queryOpConfig);
            var results = await tableQuery.GetRemainingAsync();

            return new QueryResults<T>
            {
                Results = results.Select(DocumentToResource<T>).ToArray(),
                NextPageStartToken = tableQuery.PaginationToken
            };
        }

        public async Task<QueryResults<TResource>> CustomQueryAsync<TResource, TParameters>(CustomQuery<TParameters> customQuery) where TResource : class
        {
            var customQueryBuilder = CustomQueryBuilderRegistry.Get<TParameters>(customQuery.Name);

            var queryOpConfig = customQueryBuilder.Build(customQuery);
            queryOpConfig.PaginationToken = customQuery.PageStartToken;

            var tableQuery = Table.Query(queryOpConfig);
            var results = await tableQuery.GetRemainingAsync();

            return new QueryResults<TResource>
            {
                Results = results.Select(DocumentToResource<TResource>).ToArray(),
                NextPageStartToken = tableQuery.PaginationToken
            };
        }

        public async Task<T> GetAsync<T>(string id) where T : class
        {
            var (partitionKey, sortKey) = ParsePartitionAndSortKeys(id);

            var document = await Table.GetItemAsync(partitionKey,
                                                    sortKey,
                                                    new GetItemOperationConfig {ConsistentRead = Options.ConsistentGet ?? false});

            return document != null ? DocumentToResource<T>(document) : null;
        }

        public async Task<T> PutAsync<T>(string id, T resource) where T : class
        {
            var (partitionKey, sortKey) = ParsePartitionAndSortKeys(id);
            await Table.PutItemAsync(ResourceToDocument(partitionKey, sortKey, resource));
            return resource;
        }

        public async Task DeleteAsync(string id)
        {
            var (partitionKey, sortKey) = ParsePartitionAndSortKeys(id);
            await Table.DeleteItemAsync(partitionKey, sortKey);
        }

        public IDocumentDatabaseMutex CreateMutex(string mutexName, string mutexHolder, TimeSpan? lockTimeout)
            => new DynamoDbMutex(Table, TableDescription, mutexName, mutexHolder, lockTimeout);
    }
}