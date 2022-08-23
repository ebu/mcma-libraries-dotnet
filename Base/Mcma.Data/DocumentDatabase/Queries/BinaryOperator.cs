using System.Linq;

namespace Mcma.Data.DocumentDatabase.Queries;

public class BinaryOperator
{
    public static readonly BinaryOperator EqualTo = new("=", false);
    public static readonly BinaryOperator NotEqualTo = new("!=", false);
    public static readonly BinaryOperator LessThan = new("<", false);
    public static readonly BinaryOperator LessThanOrEqualTo = new("<=", false);
    public static readonly BinaryOperator GreaterThan = new(">", false);
    public static readonly BinaryOperator GreaterThanOrEqualTo = new(">=", false);
        
    public static readonly string[] Operators = { EqualTo, NotEqualTo, LessThan, LessThanOrEqualTo, GreaterThan, GreaterThanOrEqualTo };

    private BinaryOperator(string @operator, bool validate = true)
    {
        if (validate && Operators.All(op => op != @operator))
            throw new McmaException($"Invalid operator '{@operator}'");

        Operator = @operator;
    }

    private string Operator { get; }

    public static implicit operator BinaryOperator(string @operator) => new(@operator);

    public static implicit operator string(BinaryOperator @operator) => @operator.Operator;

    public override string ToString() => this;
}