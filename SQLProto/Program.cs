using SQLProto.Parser.Queries;
using System;

namespace SQLProto
{
    class Program
    {
        static void Main(string[] args)
        {
            var testQuery = "Select 1,2,3";
            var parser = new Parser.Parser(testQuery);
            var parsedQuery=parser.ReadQuery();
            var schema = (parsedQuery as Select).GetSchema();
            var row= (parsedQuery as Select).ExecuteRow();
        }
    }
}
