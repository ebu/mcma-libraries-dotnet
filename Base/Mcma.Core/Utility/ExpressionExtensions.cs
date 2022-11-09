using System.Linq.Expressions;
using System.Reflection;

namespace Mcma.Utility;

public static class ExpressionExtensions
{
    /// <summary>
    /// Gets the <see cref="PropertyInfo"/> referenced by an <see cref="Expression{TDelegate}"/>
    /// </summary>
    /// <param name="expression">The expression referencing a property</param>
    /// <typeparam name="TObject">The type of object the expression references</typeparam>
    /// <returns>The <see cref="PropertyInfo"/> for the referenced property</returns>
    /// <exception cref="McmaException">Thrown if the provided expression is not MemberExpression that references a property</exception>
    public static PropertyInfo GetProperty<TObject>(this Expression<Func<TObject, object>> expression)
    {
        var body = expression.Body;
        if (body is UnaryExpression unary)
            body = unary.NodeType == ExpressionType.Convert
                       ? unary.Operand
                       : throw new McmaException($"Unexpected unary expression in {expression}");

        if (!(body is MemberExpression member))
            throw new McmaException($"Expression {expression} does not refer to a property on type {typeof(TObject)}");
        if (!(member.Member is PropertyInfo property))
            throw new McmaException($"Expression {expression} does not refer to a property on type {typeof(TObject)}");

        return property;
    }

    /// <summary>
    /// Gets the <see cref="PropertyInfo"/> referenced by an <see cref="Expression{TDelegate}"/>
    /// </summary>
    /// <param name="expression">The expression referencing a property</param>
    /// <typeparam name="TObject">The type of object the expression references</typeparam>
    /// <typeparam name="TProp">The type of the property returned by the expression</typeparam>
    /// <returns>The <see cref="PropertyInfo"/> for the referenced property</returns>
    /// <exception cref="McmaException">Thrown if the provided expression is not MemberExpression that references a property</exception>
    public static PropertyInfo GetProperty<TObject, TProp>(this Expression<Func<TObject, TProp>> expression)
    {
        var body = expression.Body;
        if (body is UnaryExpression unary)
            body = unary.NodeType == ExpressionType.Convert
                       ? unary.Operand
                       : throw new McmaException($"Unexpected unary expression in {expression}");

        if (!(body is MemberExpression member))
            throw new McmaException($"Expression {expression} does not refer to a property on type {typeof(TObject)}");
        if (!(member.Member is PropertyInfo property))
            throw new McmaException($"Expression {expression} does not refer to a property on type {typeof(TObject)}");

        return property;
    }

    /// <summary>
    /// Gets the name of a property referenced by an <see cref="Expression{TDelegate}"/>
    /// </summary>
    /// <param name="expression">The expression referencing a property</param>
    /// <typeparam name="TObject">The type of object the expression references</typeparam>
    /// <returns>The name of the referenced property</returns>
    public static string GetPropertyName<TObject>(this Expression<Func<TObject, object>> expression)
        => expression.GetProperty().Name;
}