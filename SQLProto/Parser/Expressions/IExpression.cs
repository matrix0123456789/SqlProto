using SQLProto.Data;
using SQLProto.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace SQLProto.Parser.Expressions
{
    public interface IExpression
    {
        DataType GetDataType((string Name, Table Table)[] tables);
        IValue Execute();
    }
}
