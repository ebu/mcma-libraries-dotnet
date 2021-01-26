using System;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Mcma.Utility
{
    public static class StreamExtensions
    {
        public static async Task<byte[]> ReadAllBytesAsync(this Stream stream)
        {
            if (stream.CanSeek)
                stream.Position = 0;
            
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            
            if (stream.CanSeek)
                stream.Position = 0;
            
            return memoryStream.ToArray();
        }
    }
    
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