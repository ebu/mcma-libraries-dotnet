using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Mcma.Utility
{
    public static class ExpressionExtensions
    {
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

        public static string GetPropertyName<TObject>(this Expression<Func<TObject, object>> expression)
            => expression.GetProperty().Name;
    }
}