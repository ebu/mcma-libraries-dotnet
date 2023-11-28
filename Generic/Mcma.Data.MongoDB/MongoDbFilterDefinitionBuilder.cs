using System;
using System.Linq;
using Mcma.Data.DocumentDatabase.Queries;
using Mcma.Utility;
using MongoDB.Driver;

namespace Mcma.Data.MongoDB;

public class MongoDbFilterDefinitionBuilder : IMongoDbFilterDefinitionBuilder
{   
    public FilterDefinition<McmaResourceDocument> Build<T>(IFilterExpression filterExpression)
        => FromFilterExpression<T>(filterExpression);
        
    private static FilterDefinition<McmaResourceDocument> FromFilterExpression<T>(IFilterExpression filterExpression)
        => filterExpression switch
        {
            FilterCriteriaGroup filterCriteriaGroup => FromFilterCriteriaGroup<T>(filterCriteriaGroup),
            FilterCriteria<T> filterCriteria => FromFilterCriteria(filterCriteria),
            _ => throw new McmaException($"Filter expression with type '{filterExpression.GetType().Name} is not supported.")
        };

    private static FilterDefinition<McmaResourceDocument> FromFilterCriteriaGroup<T>(FilterCriteriaGroup filterCriteriaGroup)
    {
        FilterDefinition<McmaResourceDocument> groupDefinition = null;
            
        foreach (var childDefinition in filterCriteriaGroup.Children.Select(FromFilterExpression<T>))
        {
            if (groupDefinition == null)
                groupDefinition = childDefinition;
            else
                groupDefinition = filterCriteriaGroup.LogicalOperator switch
                {
                    var op when op == LogicalOperator.And => Builders<McmaResourceDocument>.Filter.And(groupDefinition, childDefinition),
                    var op when op == LogicalOperator.Or => Builders<McmaResourceDocument>.Filter.Or(groupDefinition, childDefinition),
                    _ => throw new ArgumentOutOfRangeException(nameof(filterCriteriaGroup),
                                                               $"Invalid logical operator '{filterCriteriaGroup.LogicalOperator}'")
                };
        }

        return groupDefinition ?? Builders<McmaResourceDocument>.Filter.Empty;
    }

    private static FilterDefinition<McmaResourceDocument> FromFilterCriteria<T>(FilterCriteria<T> filterCriteria)
    {
        var buildMethod = MongoDbFilterMethodHelper.GetFilterMethod(filterCriteria.Property.PropertyType);
        return (FilterDefinition<McmaResourceDocument>)buildMethod.Invoke(null,
                                                                          [
                                                                              filterCriteria.Property.Name, filterCriteria.Operator,
                                                                              filterCriteria.PropertyValue
                                                                          ]);
    }
        
    internal static FilterDefinition<McmaResourceDocument> CreateBinaryOperationFilter<TProp>(string propertyName, BinaryOperator @operator, object value)
    {
        var field = new StringFieldDefinition<McmaResourceDocument, TProp>(
            $"{nameof(McmaResourceDocument.Resource).PascalCaseToCamelCase()}.{propertyName.PascalCaseToCamelCase()}");
        var typedValue = (TProp)value;
            
        return @operator switch
        {
            var op when op == BinaryOperator.EqualTo => Builders<McmaResourceDocument>.Filter.Eq(field, typedValue),
            var op when op == BinaryOperator.NotEqualTo => Builders<McmaResourceDocument>.Filter.Ne(field, typedValue),
            var op when op == BinaryOperator.GreaterThan => Builders<McmaResourceDocument>.Filter.Gt(field, typedValue),
            var op when op == BinaryOperator.GreaterThanOrEqualTo => Builders<McmaResourceDocument>.Filter.Gte(field, typedValue),
            var op when op == BinaryOperator.LessThan => Builders<McmaResourceDocument>.Filter.Lt(field, typedValue),
            var op when op == BinaryOperator.LessThanOrEqualTo => Builders<McmaResourceDocument>.Filter.Lte(field, typedValue),
            _ => throw new ArgumentOutOfRangeException(nameof(@operator), $"Invalid binary operator '{@operator}'")
        };
    }
}