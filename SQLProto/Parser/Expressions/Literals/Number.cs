using SQLProto.Data;
using SQLProto.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace SQLProto.Parser.Expressions.Literals
{
    class Number : IExpression
    {
        public string Integer = "";
        public string Fraction = "";
        public bool hasDot = false;
        public Number? Exponent = null;

        public IValue Execute()
        {
            return new Integer(long.Parse(Integer));
        }

        public DataType GetDataType((string Name, Table Table)[] tables)
        {
            return DataType.Types.Integer;
        }
    }
}
