using Mcma.Data.DocumentDatabase.Queries;
using MongoDB.Driver;

namespace Mcma.MongoDb
{
    public interface IMongoDbFilterDefinitionBuilder
    {
        FilterDefinition<McmaResourceDocument> Build<T>(IFilterExpression<T> filterExpression);
    }
}