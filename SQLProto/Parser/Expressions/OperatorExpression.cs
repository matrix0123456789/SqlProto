using SQLProto.Data;
using SQLProto.Parser.Expressions.Operators;
using SQLProto.Schema;

namespace SQLProto.Parser.Expressions
{
    public abstract class OperatorExpression : IExpression
    {
        public abstract OperatorDefinition Definition { get; }

        public static OperatorExpression Create(OperatorDefinition definition, IExpression? Left, IExpression? Right)
        {
            if (definition == OperatorDefinition.Add)
                return new Add(Left, Right);
            else
                return null;
        }

        public abstract DataType GetDataType((string Name, Table Table)[] tables);
        public abstract IValue Execute((string Name, Table Table)[] tables, IValue[][] rowSource);
    }
}