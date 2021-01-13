using System.Linq;
using Google.Cloud.Firestore;
using Mcma.Data.DocumentDatabase.Queries;

namespace Mcma.GoogleCloud.Firestore
{
    public class FirestoreQueryBuilder : IFirestoreQueryBuilder
    {
        private Query AddFilterToQuery<T>(Query firestoreQuery, IFilterExpression<T> filterExpression)
            => filterExpression is FilterCriteriaGroup<T> filterCriteriaGroup
                   ? AddFilterCriteriaGroupToQuery(firestoreQuery, filterCriteriaGroup)
                   : AddFilterCriteriaToQuery(firestoreQuery, (FilterCriteria<T>)filterExpression);
        
        private Query AddFilterCriteriaToQuery<T>(Query firestoreQuery, FilterCriteria<T> filterCriteria)
            => filterCriteria.Operator switch
            {
                var op when op == BinaryOperator.EqualTo => firestoreQuery.WhereEqualTo(filterCriteria.Property.Name, filterCriteria.PropertyValue),
                var op when op == BinaryOperator.NotEqualTo => firestoreQuery.WhereNotEqualTo(filterCriteria.Property.Name, filterCriteria.PropertyValue),
                var op when op == BinaryOperator.LessThan => firestoreQuery.WhereLessThan(filterCriteria.Property.Name, filterCriteria.PropertyValue),
                var op when op == BinaryOperator.LessThanOrEqualTo => firestoreQuery.WhereLessThanOrEqualTo(filterCriteria.Property.Name, filterCriteria.PropertyValue),
                var op when op == BinaryOperator.GreaterThan => firestoreQuery.WhereGreaterThan(filterCriteria.Property.Name, filterCriteria.PropertyValue),
                var op when op == BinaryOperator.GreaterThanOrEqualTo => firestoreQuery.WhereGreaterThanOrEqualTo(filterCriteria.Property.Name, filterCriteria.PropertyValue),
                _ => throw new McmaException($"Unrecognized binary operator '{filterCriteria.Operator}'")
            };

        private Query AddFilterCriteriaGroupToQuery<T>(Query firestoreQuery, FilterCriteriaGroup<T> filterCriteriaGroup)
        {
            if (filterCriteriaGroup.LogicalOperator == LogicalOperator.Or) {
                throw new McmaException(
                    "Firestore does not currently support the logical or (||) operator. When possible, split your query into parts and merge the results.\n" +
                    "For more information, see https://firebase.google.com/docs/firestore/query-data/queries.");
            }

            return filterCriteriaGroup.Children.Aggregate(firestoreQuery, AddFilterToQuery);
        }
        
        public Query Build<T>(Query firestoreQuery, Query<T> query)
        {
            if (query.FilterExpression != null)
                firestoreQuery = AddFilterToQuery(firestoreQuery, query.FilterExpression);

            if (query.PageSize.HasValue)
                firestoreQuery = firestoreQuery.Limit(query.PageSize.Value);
            
            if (query.PageStartToken != null)
                firestoreQuery = firestoreQuery.StartAfter(query.PageStartToken);

            return firestoreQuery;
        }
    }
}