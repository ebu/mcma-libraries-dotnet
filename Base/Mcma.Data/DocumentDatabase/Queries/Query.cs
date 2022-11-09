namespace Mcma.Data.DocumentDatabase.Queries;

public class Query<T>
{
    public string Path { get; set; }
        
    public IFilterExpression FilterExpression { get; set; }

    public int? PageSize { get; set; }
        
    public string PageStartToken { get; set; }
        
    public string SortBy { get; set; }

    public bool SortAscending { get; set; } = true;

    public Query<T> AddFilterExpression(IFilterExpression filterExpression)
    {
        FilterExpression =
            FilterExpression != null
                ? new FilterCriteriaGroup {Children = new[] {FilterExpression, filterExpression}, LogicalOperator = LogicalOperator.And}
                : filterExpression;
            
        return this;
    }
}