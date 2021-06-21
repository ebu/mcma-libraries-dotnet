using Mcma.Data.DocumentDatabase.Queries;
using MongoDB.Driver;

namespace Mcma.Data.MongoDB
{
    public interface IMongoDbFilterDefinitionBuilder
    {
        FilterDefinition<McmaResourceDocument> Build<T>(IFilterExpression<T> filterExpression);
    }
}