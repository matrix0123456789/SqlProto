using System;
using System.Collections.Generic;
using System.Text;

namespace SQLProto.Parser.Expressions.Literals
{
    public class NamedExpression
    {
        public string Name;
        public IExpression Expression;
        public NamedExpression(string name, IExpression expression)
        {
            Name = name;
            Expression = expression;
        }
    }
}
