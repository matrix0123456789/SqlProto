using System.Collections.Generic;
using System.Linq;
using SQLProto.Parser.Expressions;
using SQLProto.Parser.Expressions.Literals;
using SQLProto.Parser.Expressions.Operators;
using SQLProto.Parser.Queries;
using SQLProto.Schema;
using Xunit;

namespace SqlProto.Tests.Parser
{
    public class ParserTest
    {
        [Fact]
        public void ReadQuery1()
        {
            var parsedInterface =new SQLProto.Parser.Parser("Select a, b, c from d").ReadQuery();
            Assert.IsType<Select>(parsedInterface);
            var parsed = parsedInterface as Select;
            Assert.Equal("a", parsed.Selects[0].Name);
            Assert.Equal("a", (parsed.Selects[0].Expression as Identifier)?.Name);
            Assert.Equal("b", parsed.Selects[1].Name);
            Assert.Equal("b", (parsed.Selects[1].Expression as Identifier)?.Name);
            Assert.Equal("c", parsed.Selects[2].Name);
            Assert.Equal("c", (parsed.Selects[2].Expression as Identifier)?.Name);
            Assert.Equal("d", parsed.From.First().TableName);
        }
        [Fact]
        public void ReadQuery2()
        {
            var parsedInterface = new SQLProto.Parser.Parser("Select 2 + 3 as result").ReadQuery();
            Assert.IsType<Select>(parsedInterface);
            var parsed = parsedInterface as Select;
            Assert.Equal("result", parsed.Selects[0].Name);
            var expression = parsed.Selects[0].Expression as Add;
            Assert.Equal("2", (expression.Left as Number)?.Integer);
            Assert.Equal("3", (expression.Right as Number)?.Integer);
        }
        [Fact]
        public void ReadQuery3()
        {
            var parsedInterface = new SQLProto.Parser.Parser("create DATABASE test").ReadQuery();
            Assert.IsType<CreateDatabase>(parsedInterface);
            var parsed = parsedInterface as CreateDatabase;
            Assert.Equal("test", parsed.Name);
        }
        [Fact]
        public void ReadQuery4()
        {
            var parsedInterface = new SQLProto.Parser.Parser("create table test (a int, b text)").ReadQuery();
            Assert.IsType<CreateTable>(parsedInterface);
            var parsed = parsedInterface as CreateTable;
            Assert.Equal("test", parsed.Name);
            Assert.Equal("a", parsed.Columns[0].Name);
            Assert.Equal(DataType.Types.Integer, parsed.Columns[0].Type.Type);
            Assert.Equal("b", parsed.Columns[1].Name);
            Assert.Equal(DataType.Types.Text, parsed.Columns[1].Type.Type);
        }
        [Fact]
        public void ReadQuery5()
        {
            var parsedInterface = new SQLProto.Parser.Parser("insert into test (a, b) VALUES (1, 2)").ReadQuery();
            Assert.IsType<Insert>(parsedInterface);
            var parsed = parsedInterface as Insert;
            Assert.Null(parsed.DatabaseName);
            Assert.Equal("test", parsed.TableName);
            Assert.Equal("a", parsed.Columns[0]);
            Assert.Equal("b", parsed.Columns[1]);
            Assert.Equal("1", (parsed.Values[0] as Number)?.Integer);
            Assert.Equal("2", (parsed.Values[1] as Number)?.Integer);
        }
    }
}