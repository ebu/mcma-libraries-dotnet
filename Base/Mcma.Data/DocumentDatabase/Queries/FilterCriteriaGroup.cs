namespace Mcma.Data.DocumentDatabase.Queries;

public class FilterCriteriaGroup : IFilterExpression
{   
    public IFilterExpression[] Children { get; set; }
    public LogicalOperator LogicalOperator { get; set; }
}