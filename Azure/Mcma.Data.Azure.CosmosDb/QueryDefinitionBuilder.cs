using System.Collections.Generic;
using System.Linq;
using Mcma.Data.DocumentDatabase.Queries;
using Mcma.Utility;
using Microsoft.Azure.Cosmos;

namespace Mcma.Data.Azure.CosmosDb;

public class QueryDefinitionBuilder : IQueryDefinitionBuilder
{
    private class SqlQuery
    {
        public string Text { get; set; } = "SELECT VALUE root FROM root";
            
        public List<object> Parameters { get; } = new();

        public string AddParameter(object parameter)
        {
            var paramName = $"@p{Parameters.Count}";
            Parameters.Add(parameter);
            return paramName;
        }
    }
        
    public QueryDefinition Build<T>(Query<T> query, string partitionKeyName)
    {
        var sqlQuery = new SqlQuery();

        var partitionKeyClause =
            query.Path != null
                ? $"root[\"{partitionKeyName}\"] = {sqlQuery.AddParameter(query.Path)}"
                : null;

        var filterClause =
            query.FilterExpression != null
                ? AddFilterExpression(sqlQuery, query.FilterExpression)
                : null;

        if (partitionKeyClause != null && filterClause != null)
            sqlQuery.Text += $" WHERE ({partitionKeyClause}) and ({filterClause})";
        else if (partitionKeyClause != null || filterClause != null)
            sqlQuery.Text += $" WHERE {partitionKeyClause ?? filterClause}";

        if (!string.IsNullOrWhiteSpace(query.SortBy))
            sqlQuery.Text += $" ORDER BY root[\"resource\"][\"{query.SortBy.PascalCaseToCamelCase()}\"] {(query.SortAscending ? "asc" : "desc")}";

        return sqlQuery.Parameters
                       .Select((p, i) => new {Name = $"@p{i}", Value = p})
                       .Aggregate(
                           new QueryDefinition(sqlQuery.Text),
                           (q, p) => q.WithParameter(p.Name, p.Value));
    }

    private static string AddFilterExpression<T>(SqlQuery sqlQuery, IFilterExpression<T> filterExpression)
        => filterExpression switch
        {
            FilterCriteriaGroup<T> filterCriteriaGroup => AddFilterCriteriaGroup(sqlQuery, filterCriteriaGroup),
            FilterCriteria<T> filterCriteria => AddFilterCriteria(sqlQuery, filterCriteria),
            _ => throw new McmaException($"Filter expression with type '{filterExpression.GetType().Name} is not supported.")
        };

    private static string AddFilterCriteriaGroup<T>(SqlQuery sqlQuery, FilterCriteriaGroup<T> filterCriteriaGroup)
        =>
            "(" +
            string.Join($" {(filterCriteriaGroup.LogicalOperator == LogicalOperator.And ? "and" : "or")} ",
                        filterCriteriaGroup.Children.Select(x => AddFilterExpression(sqlQuery, x))) +
            ")";

    private static string AddFilterCriteria<T>(SqlQuery sqlQuery, FilterCriteria<T> filterCriteria)
        =>
            $"root[\"resource\"][\"{filterCriteria.Property.Name.PascalCaseToCamelCase()}\"] {filterCriteria.Operator} {sqlQuery.AddParameter(filterCriteria.PropertyValue)}";
}