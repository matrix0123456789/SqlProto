using SQLProto.Parser.Expressions.Literals;
using System;
using System.Collections.Generic;
using System.Text;

namespace SQLProto.Parser.Queries
{
    class Select : IQuery
    {
        public List<NamedExpression> Selects = new List<NamedExpression>();
    }
}
