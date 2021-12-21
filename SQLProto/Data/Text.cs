using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SQLProto.Data
{
    struct  Text : IValue
    {
        private string _value;

        public Text(string value)
        {
            _value = value;
        }

        public override string ToString()
        {
            return _value;
        }


        public void Write(Stream stream)
        {
            (new BinaryWriter(stream)).Write(_value?[0]??'?');//tmp
        }

        public static IValue Deserialize(Stream stream)
        {
            return new Text((new BinaryReader(stream)).ReadChar().ToString());//tmp
        }
    }
}
