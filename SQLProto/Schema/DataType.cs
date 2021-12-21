using SQLProto.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SQLProto.Schema
{
    public class DataType
    {
        public Types Type { get; set; }

        private DataType()
        {
        }

        public DataType(Types type)
        {
            Type = type;
        }

        public enum Types
        {
            Integer,
            Decimal,
            Text
        }

        static public implicit operator DataType(Types type)
        {
            return new DataType(type);
        }

        static public explicit operator Types(DataType type)
        {
            return type.Type;
        }

        internal IValue GetDefault()
        {
            return Type switch
            {
                Types.Integer => new Data.Integer(),
                Types.Decimal => new Data.Decimal(),
                Types.Text => new Data.Text(),
            };
        }
    }
}