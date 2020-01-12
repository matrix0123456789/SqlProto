using System;
using System.Collections.Generic;
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
    }
}