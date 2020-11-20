﻿using Amazon.DynamoDBv2.DocumentModel;
using Mcma.Data.DocumentDatabase.Queries;

namespace Mcma.Aws.DynamoDb
{
    public interface IFilterExpressionBuilder
    {
        Expression Build<T>(IFilterExpression<T> filterExpression);
    }
}