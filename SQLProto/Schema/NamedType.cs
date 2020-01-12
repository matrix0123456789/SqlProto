using System;
using System.Collections.Generic;
using System.Text;

namespace SQLProto.Schema
{
    public class NamedType
    {
        public string Name;
        public DataType Type;

        public NamedType(string name, DataType type)
        {
            Name = name;
            Type = type;
        }
    }
}