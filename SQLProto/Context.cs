using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SQLProto.Data;
using SQLProto.Parser.Queries;
using SQLProto.Schema;

namespace SQLProto
{
    public class Context
    {
        public string DefaultDB;

        async public Task<(IEnumerable<NamedType> schema, List<IValue[]> data)> ExecuteQuery(string query)
        {
            var parser = new Parser.Parser(query);
            var parsedQuery = parser.ReadQuery();
            var schema = (parsedQuery as Select).GetSchema(this);
            var tableDataAsync = schema.tables.Select(async t => (await t.Table.GetAllData()).ToArray());
            var tableData = await Task.WhenAll(tableDataAsync);
            var rows = CartesianProduck(tableData);
            var data=new List<IValue[]>();
            foreach (var rowSource in rows)
            {
                var row = (parsedQuery as Select).ExecuteRow(this, rowSource);
                data.Add(row);
            }

            return (schema.columns, data);
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