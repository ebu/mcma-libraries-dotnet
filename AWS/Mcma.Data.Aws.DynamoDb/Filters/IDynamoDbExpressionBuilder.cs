using Amazon.DynamoDBv2.DocumentModel;
using Mcma.Data.DocumentDatabase.Queries;

namespace Mcma.Data.Aws.DynamoDb.Filters;

public interface IDynamoDbExpressionBuilder
{
    Expression Build<T>(IFilterExpression filterExpression);
}