using System;

namespace SQLProto
{
    class Program
    {
        static void Main(string[] args)
        {
            var testQuery = "Select 2+2";
            var parser = new Parser.Parser(testQuery);
            var parsedQuery=parser.ReadQuery();
        }
    }
}
