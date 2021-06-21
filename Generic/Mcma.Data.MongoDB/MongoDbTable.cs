using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mcma.Data.DocumentDatabase;
using Mcma.Data.DocumentDatabase.Queries;
using Mcma.Data.DocumentDatabase.Queries.Custom;
using Mcma.Model;
using Mcma.Serialization;
using Mcma.Utility;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Mcma.Data.MongoDB
{
    public class MongoDbTable : IDocumentDatabaseTable
    {   
        public MongoDbTable(ICustomQueryBuilderRegistry<FilterDefinition<McmaResourceDocument>> customQueryBuilderRegistry,
                            IMongoDbFilterDefinitionBuilder filterDefinitionBuilder,
                            IOptions<MongoDbTableOptions> options)
        {
            CustomQueryBuilderRegistry = customQueryBuilderRegistry ?? throw new ArgumentNullException(nameof(customQueryBuilderRegistry));
            FilterDefinitionBuilder = filterDefinitionBuilder ?? throw new ArgumentNullException(nameof(filterDefinitionBuilder));
            Options = options.Value ?? new MongoDbTableOptions();

            if (string.IsNullOrWhiteSpace(Options.ConnectionString))
                throw new McmaException("MongoDB connection string not provided.");

            Client = new MongoClient(Options.ConnectionString);
            Database = Client.GetDatabase(Options.DatabaseName);
            Collection = Database.GetCollection<McmaResourceDocument>(Options.CollectionName);
        }
        
        private ICustomQueryBuilderRegistry<FilterDefinition<McmaResourceDocument>> CustomQueryBuilderRegistry { get; }

        private IMongoDbFilterDefinitionBuilder FilterDefinitionBuilder { get; }

        private MongoDbTableOptions Options { get; }
        
        private MongoClient Client { get; }
        
        private IMongoDatabase Database { get; }
        
        private IMongoCollection<McmaResourceDocument> Collection { get; }

        private static string ParsePath(string id)
        {
            var lastSlashIndex = id.LastIndexOf('/');
            return lastSlashIndex > 0 ? id.Substring(0, lastSlashIndex) : string.Empty;
        }

        private static McmaResourceDocument ResourceToDocument<T>(string id, T resource) where T : class
            => new() { Id = id, Path = ParsePath(id), Resource = resource.ToMcmaJsonObject() };

        private static T DocumentToResource<T>(McmaResourceDocument document) where T : class
            => document.Resource?.ToMcmaObject<T>();

        public async Task<QueryResults<T>> QueryAsync<T>(Query<T> query) where T : class
        {
            var filters = new List<FilterDefinition<McmaResourceDocument>> {Builders<McmaResourceDocument>.Filter.Eq(x => x.Path, query.Path)};
            if (query.FilterExpression != null)
                filters.Add(FilterDefinitionBuilder.Build(query.FilterExpression));
            
            var filterDefinition = Builders<McmaResourceDocument>.Filter.And(filters.ToArray());
                
            var find = Collection.Find(filterDefinition);

            if (int.TryParse(query.PageStartToken, out var startIndex))
                find = find.Skip(startIndex);

            if (query.PageSize.HasValue)
                find = find.Limit(query.PageSize.Value);

            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                var sortByFieldDefinition = new StringFieldDefinition<McmaResourceDocument>($"resource.{query.SortBy.PascalCaseToCamelCase()}");
                find = find.Sort(query.SortAscending
                                     ? Builders<McmaResourceDocument>.Sort.Ascending(sortByFieldDefinition)
                                     : Builders<McmaResourceDocument>.Sort.Descending(sortByFieldDefinition));
            }

            var count = await find.CountDocumentsAsync();
            var cursor = await find.ToCursorAsync();
            var results = await cursor.ToListAsync();

            return new QueryResults<T>
            {
                Results = results.Select(DocumentToResource<T>).ToArray(),
                NextPageStartToken = (startIndex + count).ToString()
            };
        }

        public async Task<QueryResults<TResource>> CustomQueryAsync<TResource, TParameters>(CustomQuery<TParameters> customQuery) where TResource : class
        {
            var customQueryBuilder = CustomQueryBuilderRegistry.Get<TParameters>(customQuery.Name);

            var filterDefinition = customQueryBuilder.Build(customQuery);

            var find = Collection.Find(filterDefinition);

            if (int.TryParse(customQuery.PageStartToken, out var startIndex))
                find.Skip(startIndex);

            var count = await find.CountDocumentsAsync();
            var cursor = await find.ToCursorAsync();
            var results = await cursor.ToListAsync();

            return new QueryResults<TResource>
            {
                Results = results.Select(DocumentToResource<TResource>).ToArray(),
                NextPageStartToken = (startIndex + count).ToString()
            };
        }

        public async Task<T> GetAsync<T>(string id) where T : class
        {
            var filter = Builders<McmaResourceDocument>.Filter.Where(x => x.Id == id);
            
            var cursor = await Collection.FindAsync(filter);
            if (!await cursor.MoveNextAsync())
                return null;

            var document = cursor.Current.FirstOrDefault();
            if (document == null)
                return null;

            return DocumentToResource<T>(document);
        }

        public async Task<T> PutAsync<T>(string id, T resource) where T : class
        {
            var filter = Builders<McmaResourceDocument>.Filter.Where(x => x.Id == id);

            var doc = ResourceToDocument(id, resource);

            await Collection.ReplaceOneAsync(filter, doc, new ReplaceOptions {IsUpsert = true});
            
            return resource;
        }

        public async Task DeleteAsync(string id)
        {
            var collection = Database.GetCollection<McmaResourceDocument>(Options.CollectionName);

            await collection.DeleteOneAsync(x => x.Id == id);
        }

        public Task<IDocumentDatabaseMutex> CreateMutexAsync(string mutexName, string mutexHolder, TimeSpan? lockTimeout = null)
        {
            return Task.FromResult<IDocumentDatabaseMutex>(new MongoDbMutex(Database.GetCollection<BsonDocument>(Options.CollectionName),
                                                                            mutexName,
                                                                            mutexHolder,
                                                                            lockTimeout));
        }
    }
}