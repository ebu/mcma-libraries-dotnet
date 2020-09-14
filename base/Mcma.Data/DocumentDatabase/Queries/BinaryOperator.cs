using System.Linq;

namespace Mcma.Data.DocumentDatabase.Queries
{
    public class BinaryOperator
    {
        public static readonly BinaryOperator EqualTo = new BinaryOperator("=", false);
        public static readonly BinaryOperator NotEqualTo = new BinaryOperator("!=", false);
        public static readonly BinaryOperator LessThan = new BinaryOperator("<", false);
        public static readonly BinaryOperator LessThanOrEqualTo = new BinaryOperator("<=", false);
        public static readonly BinaryOperator GreaterThan = new BinaryOperator(">", false);
        public static readonly BinaryOperator GreaterThanOrEqualTo = new BinaryOperator(">=", false);
        
        public static readonly string[] Operators = { EqualTo, NotEqualTo, LessThan, LessThanOrEqualTo, GreaterThan, GreaterThanOrEqualTo };

        private BinaryOperator(string @operator, bool validate = true)
        {
            if (validate && Operators.All(op => op != @operator))
                throw new McmaException($"Invalid operator '{@operator}'");

            Operator = @operator;
        }

        private string Operator { get; }

        public static implicit operator BinaryOperator(string @operator) => new BinaryOperator(@operator);

        public static implicit operator string(BinaryOperator @operator) => @operator.Operator;

        public override string ToString() => this;
    }
}