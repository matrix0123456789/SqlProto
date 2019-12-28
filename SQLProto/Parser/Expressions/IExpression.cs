using SQLProto.Data;
using SQLProto.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace SQLProto.Parser.Expressions
{
    public interface IExpression
    {
        DataType GetDataType();
        IValue Execute();
    }
}
