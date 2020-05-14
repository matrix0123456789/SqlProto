using System;
using System.Linq;
using SQLProto.Data;
using SQLProto.Schema;

namespace SQLProto.Parser.Expressions
{
    public class Identifier : IExpression
    {
        public Identifier(string name, string table=null)
        {
            Name = name;
            Table = table;
        }

        public string Name { get; set; }
        public string Table { get; set; }

        public DataType GetDataType((string Name, Table Table)[] tables)
        {
            var table = tables.First(t => Table == null || t.Name == Table).Table;
            return table.Columns.First(c => c.Name == Name).Type;
        }

        public IValue Execute((string Name, Table Table)[] tables, IValue[][] rowSource)
        {
            for (var i = 0; i < tables.Length; i++)
            {
                var t = tables[i];
                if (Table == null || t.Name == Table)
                {
                    for (var j = 0; j < t.Table.Columns.Count(); j++)
                    {
                        var column = t.Table.Columns.ToArray()[j];
                        if (column.Name == Name)
                            return rowSource[i][j];
                    }
                }
            }
           throw new Exception("not found");
        }
    }
}