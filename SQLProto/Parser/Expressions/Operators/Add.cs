using SQLProto.Data;
using SQLProto.Schema;

namespace SQLProto.Parser.Expressions.Operators
{
    public class Add:OperatorExpression
    {
        public Add(IExpression left, IExpression right)
        {
            this.Left = left;
            this.Right = right;
        }

        public IExpression Right { get; }

        public IExpression Left { get; }
        
        public override OperatorDefinition Definition => OperatorDefinition.Add;
        public override DataType GetDataType((string Name, Table Table)[] tables)
        {
            var left = Left.GetDataType(tables);
            var right = Right.GetDataType(tables);
            return DataType.Types.Decimal;
        }

        public override IValue Execute((string Name, Table Table)[] tables, IValue[][] rowSource)
        {
            var left = Left.Execute(tables, rowSource);
            var right = Right.Execute(tables, rowSource);
            Data.Decimal leftD;
            Data.Decimal rightD;
            if (left is Data.Integer)
            {
                leftD =(Data.Integer) left;
            }
            else if (left is Data.Decimal)
            {
                leftD =(Data.Decimal) left;
            }
            else
            {
                throw new FormatError();
            }
            if (right is Data.Integer)
            {
                rightD =(Data.Integer) right;
            }
            else if (right is Data.Decimal)
            {
                rightD =(Data.Decimal) right;
            }
            else
            {
                throw new FormatError();
            }
            return leftD+rightD;
        }
    }
}