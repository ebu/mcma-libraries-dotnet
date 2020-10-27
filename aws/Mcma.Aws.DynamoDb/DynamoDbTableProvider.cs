using System;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Mcma.Data;
using Mcma.Data.DocumentDatabase.Queries;
using Microsoft.Extensions.Options;

namespace Mcma.Aws.DynamoDb
{
    public class DynamoDbTableProvider : IDocumentDatabaseTableProvider
    {
        public DynamoDbTableProvider(ICustomQueryBuilderRegistry<QueryOperationConfig> customQueryBuilderRegistry, 
                                     IAttributeMapper attributeMapper,
                                     ITableDescriptionProvider tableDescriptionProvider,
                                     IFilterExpressionBuilder filterExpressionBuilder,
                                     IOptions<DynamoDbTableProviderOptions> providerOptions)
        {
            CustomQueryBuilderRegistry = customQueryBuilderRegistry ?? throw new ArgumentNullException(nameof(customQueryBuilderRegistry));
            AttributeMapper = attributeMapper ?? throw new ArgumentNullException(nameof(attributeMapper));
            TableDescriptionProvider = tableDescriptionProvider ?? throw new ArgumentNullException(nameof(tableDescriptionProvider));
            FilterExpressionBuilder = filterExpressionBuilder ?? throw new ArgumentNullException(nameof(filterExpressionBuilder));
            
            Options = providerOptions?.Value ?? new DynamoDbTableProviderOptions();
            
            DynamoDb = new AmazonDynamoDBClient(Options.Credentials, Options.Config);
        }
        
        private ICustomQueryBuilderRegistry<QueryOperationConfig> CustomQueryBuilderRegistry { get; }

        private IAttributeMapper AttributeMapper { get; }

        private ITableDescriptionProvider TableDescriptionProvider { get; }

        private IFilterExpressionBuilder FilterExpressionBuilder { get; }

        private DynamoDbTableProviderOptions Options { get; }
        
        private IAmazonDynamoDB DynamoDb { get; }

        public async Task<IDocumentDatabaseTable> GetAsync(string tableName)
            => new DynamoDbTable(CustomQueryBuilderRegistry,
                                 AttributeMapper,
                                 FilterExpressionBuilder,
                                 DynamoDb,
                                 await TableDescriptionProvider.GetTableDescriptionAsync(tableName),
                                 Options);
    }
}