using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SQLProto.Data
{
    struct Decimal : IValue
    {
        private decimal _value;

        public Decimal(decimal value)
        {
            _value = value;
        }

        public override string ToString()
        {
            return _value.ToString();
        }

        public void Write(Stream stream)
        {
            (new BinaryWriter(stream)).Write(_value);
        }

        static public Decimal operator +(Decimal a, Decimal b)
        {
            return new Decimal(a._value + b._value);
        }

        static public Decimal operator -(Decimal a, Decimal b)
        {
            return new Decimal(a._value - b._value);
        }

        static public Decimal operator *(Decimal a, Decimal b)
        {
            return new Decimal(a._value * b._value);
        }

        static public Decimal operator /(Decimal a, Decimal b)
        {
            return new Decimal(a._value / b._value);
        }

        public static IValue Deserialize(Stream stream)
        {
            return new Decimal((new BinaryReader(stream)).ReadDecimal());
        }
    }
}