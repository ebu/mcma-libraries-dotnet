using System.Linq;

namespace Mcma.Data.DocumentDatabase.Queries;

public class BinaryOperator
{
    public static readonly BinaryOperator EqualTo              = new("=");
    public static readonly BinaryOperator NotEqualTo           = new("!=");
    public static readonly BinaryOperator LessThan             = new("<");
    public static readonly BinaryOperator LessThanOrEqualTo    = new("<=");
    public static readonly BinaryOperator GreaterThan          = new(">");
    public static readonly BinaryOperator GreaterThanOrEqualTo = new(">=");
        
    public static readonly string[] Operators = { EqualTo, NotEqualTo, LessThan, LessThanOrEqualTo, GreaterThan, GreaterThanOrEqualTo };

    private BinaryOperator(string @operator)
    {
        Operator = @operator;
    }

    private string Operator { get; }

    private static BinaryOperator CreateWithValidation(string @operator)
    {
        if (Operators.All(op => op != @operator))
            throw new McmaException($"Invalid operator '{@operator}'");

        return new BinaryOperator(@operator);
    }

    public static implicit operator BinaryOperator(string @operator) => CreateWithValidation(@operator);

    public static implicit operator string(BinaryOperator @operator) => @operator.Operator;

    public override string ToString() => this;
}