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
    }
}
