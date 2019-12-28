using SQLProto.Data;
using SQLProto.Parser.Expressions.Literals;
using SQLProto.Schema;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SQLProto.Parser.Queries
{
    class Select : IQuery
    {
        public List<NamedExpression> Selects = new List<NamedExpression>();
        public IEnumerable<NamedType> GetSchema()
        {
            return Selects.Select(x => new NamedType(x.Name, x.Expression.GetDataType()));
        }

        public IEnumerable<IValue> ExecuteRow()
        {
            return Selects.Select(x => x.Expression.Execute());
        }
    }
}
