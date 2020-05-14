using System;
using System.Collections.Generic;
using System.Text;

namespace SQLProto.Schema
{
    public class NamedType
    {
        public string Name { get; set; }
        public DataType Type { get; set; }

        private NamedType()
        {
        }

        public NamedType(string name, DataType type)
        {
            Name = name;
            Type = type;
        }
    }
}