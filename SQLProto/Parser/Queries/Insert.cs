using System.Collections.Generic;
using SQLProto.Parser.Expressions;

namespace SQLProto.Parser.Queries
{
    public class Insert: IQuery
    {
        public string? DatabaseName;
        public string TableName;
        public List<string> Columns;
        public List<IExpression> Values;
    }
}