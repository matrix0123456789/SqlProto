using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SQLProto.Data;
using SQLProto.Parser.Queries;
using SQLProto.Schema;
using SQLProto.Storage;

namespace SQLProto
{
    public class Context
    {
        public string DefaultDB;

        async public Task<(IEnumerable<NamedType> schema, List<IValue[]> data)> ExecuteQuery(string query)
        {
            var parser = new Parser.Parser(query);
            var parsedQuery = parser.ReadQuery();
            if (parsedQuery is Select)
            {
                var schema = (parsedQuery as Select).GetSchema(this);
                var tableDataAsync = schema.tables.Select(async t => (await t.Table.GetAllData()).ToArray());
                var tableData = await Task.WhenAll(tableDataAsync);
                var rows = CartesianProduck(tableData);
                var data = new List<IValue[]>();
                foreach (var rowSource in rows)
                {
                    var row = (parsedQuery as Select).ExecuteRow(this, rowSource);
                    data.Add(row);
                }

                return (schema.columns, data);
            }
            else if (parsedQuery is Insert insert)
            {
                var table = Database.AllDatabases[this.DefaultDB].Tables[insert.TableName];
                var row = table.Columns.Select(c => c.Type.GetDefault()).ToArray();
                var columns = table.Columns.ToArray();
                var i = 0;
                foreach (var colName in insert.Columns)
                {
                    var columnIndex = Array.FindIndex(columns, c => c.Name == colName);
                    var value = insert.Values[i];
                    var valueExecuted = value.Execute(Array.Empty<(string,Table)>(), Array.Empty<IValue[]>());
                    if (valueExecuted.GetType() != row[columnIndex].GetType())
                        throw new NotImplementedException();

                    row[columnIndex] = valueExecuted;
                    i++;
                }
                table.Insert(row);
                return default;
            }
            else
            {
                if (parsedQuery is CreateDatabase database)
                {
                    Database.Create(database.Name);
                }
                else if (parsedQuery is CreateTable table)
                {
                    Database.AllDatabases[this.DefaultDB].CreateTable(table.Name, table.Columns);
                }

                return default;
            }
        }

        public IEnumerable<IValue[][]> CartesianProduck(IValue[][][] tableData)
        {
            var count = tableData.Length;
            var index = new Multindex(tableData.Select(x => x.LongCount()).ToArray());
            do
            {
                var row = new IValue[count][];
                for (var i = 0; i < count; i++)
                {
                    row[i] = tableData[i][index.Index[i]];
                }

                yield return row;
            } while (index.Increment());
        }

        class Multindex
        {
            public long[] Index;

            public Multindex(long[] Max)
            {
                this.Max = Max;
                this.Index = new long[Max.Length];
            }

            public long[] Max { get; set; }

            public bool Increment()
            {
                if (Index.Length == 0)
                    return false;

                for (int i = Index.Length - 1; i >= 0; i--)
                {
                    Index[i]++;
                    if (Index[i] >= Max[i])
                    {
                        Index[i] = 0;
                        if (i == 0)
                            return false;
                    }
                    else
                    {
                        return true;
                    }
                }

                return true;
            }
        }
    }
}