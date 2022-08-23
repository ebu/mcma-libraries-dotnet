using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Mcma.Data.Aws.DynamoDb.AttributeMapping;
using Mcma.Data.Aws.DynamoDb.Filters;
using Mcma.Data.Aws.DynamoDb.TableDescription;
using Mcma.Data.DocumentDatabase;
using Mcma.Data.DocumentDatabase.Queries;
using Mcma.Data.DocumentDatabase.Queries.Custom;
using Mcma.Model;
using Mcma.Serialization;
using Mcma.Utility;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Mcma.Data.Aws.DynamoDb;

public class DynamoDbTable : IDocumentDatabaseTable
{
    public DynamoDbTable(ICustomQueryBuilderRegistry<QueryOperationConfig> customQueryBuilderRegistry, 
                         IAttributeMapper attributeMapper,
                         ITableDescriptionProvider tableDescriptionProvider,
                         IDynamoDbExpressionBuilder expressionBuilder,
                         IOptions<DynamoDbTableOptions> providerOptions)
    {
        CustomQueryBuilderRegistry = customQueryBuilderRegistry ?? throw new ArgumentNullException(nameof(customQueryBuilderRegistry));
        AttributeMapper = attributeMapper ?? throw new ArgumentNullException(nameof(attributeMapper));
        TableDescriptionProvider = tableDescriptionProvider ?? throw new ArgumentNullException(nameof(tableDescriptionProvider));
        ExpressionBuilder = expressionBuilder ?? throw new ArgumentNullException(nameof(expressionBuilder));
            
        Options = providerOptions?.Value ?? new DynamoDbTableOptions();
            
        DynamoDb = new AmazonDynamoDBClient(Options.Credentials, Options.Config);
        Table = Table.LoadTable(DynamoDb, Options.TableName);
    }
        
    private ICustomQueryBuilderRegistry<QueryOperationConfig> CustomQueryBuilderRegistry { get; }

    private IAttributeMapper AttributeMapper { get; }

    private ITableDescriptionProvider TableDescriptionProvider { get; }

    private IDynamoDbExpressionBuilder ExpressionBuilder { get; }

    private DynamoDbTableOptions Options { get; }
        
    private IAmazonDynamoDB DynamoDb { get; }

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

    private Document ResourceToDocument<T>(DynamoDbTableDescription tableDescription, string partitionKey, string sortKey, T resource)
    {
        var resourceJson = resource.ToMcmaJson().RemoveEmptyStrings();
            
        var item = new JObject
        {
            [tableDescription.KeyNames.Partition] = partitionKey,
            [tableDescription.KeyNames.Sort] = sortKey,
            [nameof(resource)] = resourceJson
        };

        foreach (var kvp in AttributeMapper.GetMappedAttributes(partitionKey, sortKey, resource))
            item[kvp.Key] = kvp.Value != null ? JToken.FromObject(kvp.Value) : JValue.CreateNull();

        return Document.FromJson(item.ToString());
    }

    public async Task<QueryResults<T>> QueryAsync<T>(Query<T> query) where T : class
    {
        var tableDescription = await TableDescriptionProvider.GetTableDescriptionAsync(DynamoDb, Options.TableName);
            
        var keyExpression =
            new Expression
            {
                ExpressionStatement = "#partitionKey = :partitionKey",
                ExpressionAttributeNames = new Dictionary<string, string> {["#partitionKey"] = tableDescription.KeyNames.Partition},
                ExpressionAttributeValues = new Dictionary<string, DynamoDBEntry> {[":partitionKey"] = query.Path}
            };

        var filterExpression = query.FilterExpression != null ? ExpressionBuilder.Build(query.FilterExpression) : null;

        var indexName = default(string);
        if (query.SortBy != null)
        {
            var matchingIndex =
                tableDescription.LocalSecondaryIndexes.FirstOrDefault(
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
            PaginationToken = query.PageStartToken?.FromBase64()
        };

        var tableQuery = Table.Query(queryOpConfig);
        var results = await tableQuery.GetNextSetAsync();

        return new QueryResults<T>
        {
            Results = results.Select(DocumentToResource<T>).ToArray(),
            NextPageStartToken = tableQuery.NextKey != null && tableQuery.NextKey.Any() ? tableQuery.PaginationToken.ToBase64() : null
        };
    }

    public async Task<QueryResults<TResource>> CustomQueryAsync<TResource, TParameters>(CustomQuery<TParameters> customQuery) where TResource : class
    {
        var customQueryBuilder = CustomQueryBuilderRegistry.Get<TParameters>(customQuery.Name);

        var queryOpConfig = customQueryBuilder.Build(customQuery);
        queryOpConfig.PaginationToken = customQuery.PageStartToken?.FromBase64();

        var tableQuery = Table.Query(queryOpConfig);
        var results = await tableQuery.GetNextSetAsync();

        return new QueryResults<TResource>
        {
            Results = results.Select(DocumentToResource<TResource>).ToArray(),
            NextPageStartToken = tableQuery.NextKey != null && tableQuery.NextKey.Any() ? tableQuery.PaginationToken.ToBase64() : null
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
        var tableDescription = await TableDescriptionProvider.GetTableDescriptionAsync(DynamoDb, Options.TableName);
        var (partitionKey, sortKey) = ParsePartitionAndSortKeys(id);
        await Table.PutItemAsync(ResourceToDocument(tableDescription, partitionKey, sortKey, resource));
        return resource;
    }

    public async Task DeleteAsync(string id)
    {
        var (partitionKey, sortKey) = ParsePartitionAndSortKeys(id);
        await Table.DeleteItemAsync(partitionKey, sortKey);
    }

    public async Task<IDocumentDatabaseMutex> CreateMutexAsync(string mutexName, string mutexHolder, TimeSpan? lockTimeout)
    {
        var tableDescription = await TableDescriptionProvider.GetTableDescriptionAsync(DynamoDb, Options.TableName);
            
        return new DynamoDbMutex(Table, tableDescription, mutexName, mutexHolder, lockTimeout);
    }
}