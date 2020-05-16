using SQLProto.Data;
using SQLProto.Parser.Expressions.Literals;
using SQLProto.Schema;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SQLProto.Parser.Queries
{
    public class Select : IQuery
    {
        public List<NamedExpression> Selects = new List<NamedExpression>();
        public List<SourceTable> From = new List<SourceTable>();
        public ((string Name, Table Table)[] tables, IEnumerable<NamedType> columns) GetSchema(Context context)
        {
            var tables = GetTables(context);
            var columns= Selects.Select(x => new NamedType(x.Name, x.Expression.GetDataType(tables)));
            return (tables, columns);
        }

        private (string Name, Table Table)[] GetTables(Context context)
        {
            var tables = From.Select(t =>
            {
                var db = Database.AllDatabases[t.DatabaseName ?? context.DefaultDB];
                return (Name: t.TableName, Table: db.Tables[t.TableName]);
            }).ToArray();
            return tables;
        }

        public IValue[] ExecuteRow(Context context, IValue[][] rowSource)
        {
            var tables = GetTables(context);
            return Selects.Select(x => x.Expression.Execute(tables, rowSource)).ToArray();
        }
    }
}
