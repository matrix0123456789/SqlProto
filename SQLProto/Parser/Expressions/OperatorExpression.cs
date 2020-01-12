using SQLProto.Data;
using SQLProto.Parser.Expressions.Operators;
using SQLProto.Schema;

namespace SQLProto.Parser.Expressions
{
    public abstract class OperatorExpression : IExpression
    {
        public OperatorDefinition Definition { get; }

        public static OperatorExpression Create(OperatorDefinition definition, IExpression? Left, IExpression? Right)
        {
            if (definition == OperatorDefinition.Add)
                return new Add(Left, Right);
            else
                return null;
        }

        public abstract DataType GetDataType();
        public abstract IValue Execute();
    }
}