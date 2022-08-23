using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Mcma.Data.DocumentDatabase.Queries.Custom;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Data.DocumentDatabase.Queries;

public static class CustomQueryServiceCollectionExtensions
{
    public static IServiceCollection AddCustomQueryBuilder<TParams, TProviderQuery, TCustomQueryBuilder>(this IServiceCollection services)
        where TCustomQueryBuilder : class, ICustomQueryBuilder<TParams, TProviderQuery>
        => services.AddSingleton<ICustomQueryBuilder<TParams, TProviderQuery>, TCustomQueryBuilder>();
}
    
public static class HelperExtensions
{
    public static IFilterExpression<T> ToFilterExpression<T>(this IEnumerable<KeyValuePair<string, object>> keyValuePairs)
    {
        return new FilterCriteriaGroup<T>
        {
            Children = keyValuePairs.Select(kvp => new FilterCriteria<T>(kvp.Key, BinaryOperator.EqualTo, kvp.Value))
                                    .ToArray<IFilterExpression<T>>(),
            LogicalOperator = LogicalOperator.And
        };
    }
        
    public static IFilterExpression<T> ToFilterExpression<T>(this IEnumerable<KeyValuePair<string, string>> keyValuePairs)
    {
        return new FilterCriteriaGroup<T>
        {
            Children = keyValuePairs.Select(kvp => new FilterCriteria<T>(kvp.Key, BinaryOperator.EqualTo, kvp.Value))
                                    .ToArray<IFilterExpression<T>>(),
            LogicalOperator = LogicalOperator.And
        };
    }
        
    public static Expression<Func<T, bool>> ToExpression<T>(this Query<T> query)
    {
        var parameter = Expression.Parameter(typeof(T));

        var pathProperty = typeof(Query<T>).GetProperty(nameof(Query<T>.Path));
            
        var pathPropertyExpr = Expression.Property(parameter, pathProperty);
        var pathValueExpr = Expression.Constant(query.Path);
        var pathEqualsExpr = Expression.Equal(pathPropertyExpr, pathValueExpr);

        var lambdaBody =
            query.FilterExpression != null
                ? Expression.And(pathEqualsExpr, query.FilterExpression.ToExpression())
                : pathEqualsExpr;

        return Expression.Lambda<Func<T, bool>>(lambdaBody, parameter);
    }

    private static Expression ToExpression<T>(this IFilterExpression<T> filterExpression)
        =>
            filterExpression switch
            {
                FilterCriteriaGroup<T> criteriaGroup => criteriaGroup.ToExpression(),
                FilterCriteria<T> criteria => criteria.ToExpression(),
                _ => throw new ArgumentException($"Unsupported filter expression type {filterExpression?.GetType()}")
            };

    private static Expression ToExpression<T>(this FilterCriteriaGroup<T> filterCriteriaGroup)
    {
        Expression groupExpr = null;
            
        foreach (var childExpression in filterCriteriaGroup.Children.Select(c => c.ToExpression()))
        {
            if (groupExpr == null)
                groupExpr = childExpression;
            else
                groupExpr = filterCriteriaGroup.LogicalOperator switch
                {
                    var op when op == LogicalOperator.And => Expression.And(groupExpr, childExpression),
                    var op when op == LogicalOperator.Or => Expression.Or(groupExpr, childExpression),
                    _ => throw new ArgumentOutOfRangeException(nameof(filterCriteriaGroup),
                                                               $"Invalid logical operator '{filterCriteriaGroup.LogicalOperator}'")
                };
        }

        return groupExpr ?? Expression.Constant(true);
    }

    private static Expression ToExpression<T>(this FilterCriteria<T> filterCriteria, Expression parameter)
    {
        var propertyExpr = Expression.Property(parameter, filterCriteria.Property.Name);
        var valueExpr = Expression.Unbox(Expression.Constant(filterCriteria.PropertyValue), filterCriteria.Property.PropertyType);
            
        return filterCriteria.Operator switch
        {
            var op when op == BinaryOperator.EqualTo => Expression.Equal(propertyExpr, valueExpr),
            var op when op == BinaryOperator.NotEqualTo => Expression.NotEqual(propertyExpr, valueExpr),
            var op when op == BinaryOperator.GreaterThan => Expression.GreaterThan(propertyExpr, valueExpr),
            var op when op == BinaryOperator.GreaterThanOrEqualTo => Expression.GreaterThanOrEqual(propertyExpr, valueExpr),
            var op when op == BinaryOperator.LessThan => Expression.LessThan(propertyExpr, valueExpr),
            var op when op == BinaryOperator.LessThanOrEqualTo => Expression.LessThanOrEqual(propertyExpr, valueExpr),
            _ => throw new ArgumentOutOfRangeException(nameof(filterCriteria), $"Invalid binary operator '{filterCriteria.Operator}'")
        };
    }
}