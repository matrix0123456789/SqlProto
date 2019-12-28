﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SQLProto.Schema
{
    public class DataType
    {
        public Types Type;

        public DataType(Types type)
        {
            Type = type;
        }

        public enum Types
        {
            Integer, Decimal
        }
        static public implicit operator DataType(Types type)
        {
            return new DataType(type);
        }
        static public explicit operator Types(DataType type)
        {
            return type.Type;
        }
    }
}