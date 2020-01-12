using System;
using System.Collections.Generic;
using System.Text;

namespace SQLProto.Data
{
    struct Integer : IValue
    {
        private long _value;

        public Integer(long value)
        {
            _value = value;
        }

        public override string ToString()
        {
            return _value.ToString();
        }

        public static implicit operator Decimal(Integer source)
        {
            return new Decimal(source._value);
        }
    }
}