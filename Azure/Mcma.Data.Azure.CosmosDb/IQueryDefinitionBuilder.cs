using Mcma.Data.DocumentDatabase.Queries;
using Microsoft.Azure.Cosmos;

namespace Mcma.Data.Azure.CosmosDb
{
    public interface IQueryDefinitionBuilder
    {
        QueryDefinition Build<T>(Query<T> query, string partitionKeyName);
    }
}