using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SQLProto.Data
{
    public interface IValue
    {
        void Write(Stream stream);
    }
}
