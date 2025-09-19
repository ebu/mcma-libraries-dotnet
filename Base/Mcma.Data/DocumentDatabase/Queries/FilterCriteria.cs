using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mcma.Utility;

namespace Mcma.Data.DocumentDatabase.Queries;

public class FilterCriteria<TDoc> : IFilterExpression
{
    public FilterCriteria(string propertyName, BinaryOperator @operator, object propertyValue)
    {
        Property = typeof(TDoc).GetProperties().FirstOrDefault(p => p.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase)) ??
                   throw new McmaException($"Property '{propertyName}' does not exist on type {typeof(TDoc).Name}.");
        Operator = @operator;
        PropertyValue = propertyValue;
            
        if (!Property.PropertyType.IsInstanceOfType(propertyValue))
            throw new McmaException($"Property {propertyName} on type {typeof(TDoc).Name} cannot be assigned a value of type {propertyValue?.GetType().Name ?? "(null)"}");
    }
        
    public PropertyInfo Property { get; }
    public BinaryOperator Operator { get; }
    public object PropertyValue { get; }
}
    
public class FilterCriteria<TDoc, TProp> : FilterCriteria<TDoc>
{
    public FilterCriteria(Expression<Func<TDoc, TProp>> propertyExpression, BinaryOperator @operator, TProp propertyValue)
        : base(propertyExpression.GetProperty().Name, @operator, propertyValue)
    {
    }
}