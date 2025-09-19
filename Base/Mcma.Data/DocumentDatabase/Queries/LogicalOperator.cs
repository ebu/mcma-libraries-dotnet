using System.Linq;

namespace Mcma.Data.DocumentDatabase.Queries;

public class LogicalOperator
{
    public static readonly LogicalOperator And = new("&&");
    public static readonly LogicalOperator Or  = new("||");
    
    public static readonly string[] Operators = [And, Or];

    private LogicalOperator(string @operator)
    {
        Operator = @operator;
    }

    private string Operator { get; }
    
    private static LogicalOperator CreateWithValidation(string @operator)
    {
        if (Operators.All(op => op != @operator))
            throw new McmaException($"Invalid operator '{@operator}'");

        return new LogicalOperator(@operator);
    }

    public static implicit operator LogicalOperator(string @operator) => CreateWithValidation(@operator);

    public static implicit operator string(LogicalOperator @operator) => @operator.Operator;
}