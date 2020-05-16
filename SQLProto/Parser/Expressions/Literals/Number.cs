using SQLProto.Data;
using SQLProto.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace SQLProto.Parser.Expressions.Literals
{
    public class Number : IExpression
    {
        public string Integer = "";
        public string Fraction = "";
        public bool hasDot = false;
        public Number? Exponent = null;

        public IValue Execute((string Name, Table Table)[] tables, IValue[][] rowSource)
        {
            return new Integer(long.Parse(Integer));
        }

        public DataType GetDataType((string Name, Table Table)[] tables)
        {
            return DataType.Types.Integer;
        }
    }
}
