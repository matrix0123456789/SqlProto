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
        public List<SourceTable> From = new List<SourceTable>();
        public IEnumerable<NamedType> GetSchema(Context context)
        {
            var tables = From.Select(t =>
            {
                var db = Database.AllDatabases[t.DatabaseName ?? context.DefaultDB];
                return (Name: t.TableName, Table: db.Tables[t.TableName]);
            }).ToArray();
            return Selects.Select(x => new NamedType(x.Name, x.Expression.GetDataType(tables)));
        }

        public IEnumerable<IValue> ExecuteRow()
        {
            return Selects.Select(x => x.Expression.Execute());
        }
    }
}
