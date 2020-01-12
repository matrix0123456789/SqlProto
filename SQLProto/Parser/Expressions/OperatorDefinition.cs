using System.Reflection.Metadata.Ecma335;

namespace SQLProto.Parser.Expressions
{
    public class OperatorDefinition
    {
        public static readonly OperatorDefinition Multiply = new OperatorDefinition("*", true,true, 1);
        public static readonly OperatorDefinition Divide = new OperatorDefinition("/", true,true, 1);
        public static readonly OperatorDefinition Add = new OperatorDefinition("+", true,true, 2);
        public static readonly OperatorDefinition Subtract = new OperatorDefinition("-", true,true, 2);
        public static readonly OperatorDefinition[] All = new[] {Multiply, Divide, Add, Subtract};

        public OperatorDefinition(string code, bool left, bool right, int precedence)
        {
            Coded = code;
            Left = left;
            Right = right;
            Precedence = precedence;
        }

        public string Coded { get; }
        public bool Left { get; }
        public bool Right { get; }
        public int Precedence { get; }
    }
}