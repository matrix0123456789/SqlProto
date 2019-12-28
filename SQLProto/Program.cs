using SQLProto.Data;
using SQLProto.Parser.Queries;
using SQLProto.Schema;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SQLProto
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                var testQuery = Console.ReadLine();
                var parser = new Parser.Parser(testQuery);
                var parsedQuery = parser.ReadQuery();
                var schema = (parsedQuery as Select).GetSchema();
                var row = (parsedQuery as Select).ExecuteRow();
                DrawTable(schema, new[] { row });
            }
        }

        private static void DrawTable(IEnumerable<NamedType> schema, IEnumerable<IEnumerable<IValue>> rows)
        {
            var schemaArray = schema.ToArray();
            var charsForOne = (Console.WindowWidth - schemaArray.Length + 1) / schemaArray.Length;
            {
                Console.WriteLine();
                var first = true;
                foreach (var col in schemaArray)
                {
                    if (!first) Console.Write("|");
                    var name = col.Name;
                    if (name.Length > charsForOne)
                    {
                        name = name.Substring(0, charsForOne);
                    }
                    while (name.Length < charsForOne)
                        name += " ";
                    Console.Write(name);
                    first = false;
                }
                Console.WriteLine();
                for (var i = 0; i < (charsForOne + 1) * schemaArray.Length - 1; i++)
                    Console.Write('-');
            }
            foreach (var row in rows)
            {
                Console.WriteLine();
                var first = true;
                foreach (var value in row)
                {
                    if (!first) Console.Write("|");
                    var cell = value.ToString();
                    if (cell.Length > charsForOne)
                    {
                        cell = cell.Substring(0, charsForOne);
                    }
                    while (cell.Length < charsForOne)
                        cell += " ";
                    Console.Write(cell);
                    first = false;
                }
            }
        }

    }
}
