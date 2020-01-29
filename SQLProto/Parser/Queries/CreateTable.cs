using System.Collections.Generic;
using SQLProto.Schema;

namespace SQLProto.Parser.Queries
{
    public class CreateTable : IQuery
    {
        public string Name { get; set; }
        public List<NamedType> Columns { get; set; } = new List<NamedType>();

        public CreateTable(string name)
        {
            Name = name;
        }
    }
}