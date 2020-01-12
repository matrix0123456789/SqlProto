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
        public override DataType GetDataType()
        {
            var left = Left.GetDataType();
            var right = Right.GetDataType();
            return DataType.Types.Decimal;
        }

        public override IValue Execute()
        {
            var left = Left.Execute();
            var right = Right.Execute();
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