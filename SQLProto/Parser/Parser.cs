using SQLProto.Parser.Expressions;
using SQLProto.Parser.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SQLProto.Parser.Expressions.Literals;
using SQLProto.Schema;
using SQLProto.Utils;

namespace SQLProto.Parser
{
    public class Parser
    {
        private string code;
        private int position;
        private static string[] keywords = {"as", "from"};
        private static char[] specialChars = {'.', ',', '(', ')'};

        public Parser(string code)
        {
            this.code = code;
        }

        public IQuery? ReadQuery(IEnumerable<string> escapeStrings = null)
        {
            IQuery finded = null;
            while (position < code.Length)
            {
                var currentChar = code[position];
                if (this.IsWhiteChar(currentChar))
                {
                    this.position++;
                    continue;
                }
                else if (this.IsAnyOfArray(escapeStrings))
                {
                    return finded;
                }
                else
                {
                    var word = this.FindWord();
                    switch (word.ToLower())
                    {
                        case "select":
                            return ReadSelect(escapeStrings);
                        case "create":
                            var type = FindWord();
                            switch (type.ToLower())
                            {
                                case "database":
                                    return ReadCreateDatabase(escapeStrings);
                                case "table":
                                    return ReadCreateTable(escapeStrings);
                                default:
                                    throw new SyntaxError("Object type unknown");
                            }

                        default:
                            throw new SyntaxError("unexpected keyword");
                    }
                }
            }

            return finded;
        }

        private CreateDatabase ReadCreateDatabase(IEnumerable<string> escapeStrings)
        {
            var name = FindWord();
            var query = new CreateDatabase(name);
            while (position < code.Length)
            {
                SkipWhiteChars();
                if (this.IsAnyOfArray(escapeStrings))
                {
                    return query;
                }

                throw new SyntaxError("");
            }

            return query;
        }

        private CreateTable ReadCreateTable(IEnumerable<string> escapeStrings)
        {
            var name = FindWord();
            var query = new CreateTable(name);

            SkipWhiteChars();
            if (IsAnyOfArray(new[] {"("}))
            {
                position++;
            }
            else
            {
                throw new SyntaxError("");
            }

            while (position < code.Length)
            {
                SkipWhiteChars();
                if (this.IsAnyOfArray(escapeStrings))
                {
                    return query;
                }

                query.Columns.Add(ReadColumnDefinition(escapeStrings.OrEmpty().Concat(new[] {",", ")"})));
            }

            return query;
        }

        private NamedType ReadColumnDefinition(IEnumerable<string> escapeStrings)
        {
            SkipWhiteChars();
            if (this.IsAnyOfArray(escapeStrings))
            {
                throw new SyntaxError("");
            }

            var name = FindWord();
            SkipWhiteChars();
            if (this.IsAnyOfArray(escapeStrings))
            {
                throw new SyntaxError("");
            }

            var type = FindWord();
            var ret = new NamedType(name, Enum.Parse<DataType.Types>(type));
            while (position < code.Length)
            {
                if (this.IsAnyOfArray(escapeStrings))
                {
                    return ret;
                }

                position++;
            }

            return ret;
        }

        Select ReadSelect(IEnumerable<string> escapeStrings = null)
        {
            escapeStrings = escapeStrings ?? new string[0];
            var query = new Select();
            query.Selects = ReadSelectFields(escapeStrings.Concat(new[] {"from", "where"}));

            while (position < code.Length)
            {
                SkipWhiteChars();
                if (this.IsAnyOfArray(escapeStrings))
                {
                    return query;
                }

                if (TryFindWord().ToLowerInvariant() == "from")
                {
                    FindWord();
                    var tableName = FindMultword();
                    if (tableName.Count == 0)
                        throw new SyntaxError("Empty table name");
                    else if (tableName.Count == 1)
                        query.From.Add(new SourceTable(tableName[0]));
                    else if (tableName.Count == 2)
                        query.From.Add(new SourceTable(tableName[1], tableName[0]));
                    else
                        throw new SyntaxError("Too much dots");
                    continue;
                }

                throw new SyntaxError("");
            }

            return query;
        }

        private List<NamedExpression> ReadSelectFields(IEnumerable<string> escapeStrings = null)
        {
            var selects = new List<NamedExpression>();
            escapeStrings = escapeStrings ?? new string[0];

            while (position < code.Length)
            {
                SkipWhiteChars();
                if (IsAnyOfArray(escapeStrings)) break;
                var itemStart = position;
                var item = readNormal(escapeStrings.Concat(new string[] {",", "as"}));
                if (item != null)
                {
                    var name = code.Substring(itemStart, position - itemStart);

                    SkipWhiteChars();
                    if (TryFindWord().ToLowerInvariant() == "as")
                    {
                        FindWord();
                        name = FindWord();
                    }

                    SkipWhiteChars();
                    if (position < code.Length && code[position] == ',')
                        this.position++;
                    selects.Add(new Expressions.Literals.NamedExpression(name, item));
                }
            }

            return selects;
        }

        IExpression readNormal(IEnumerable<string> escapeStrings = null, int escapePrecedence = 0)
        {
            IExpression? finded = null; //todo zrobić stack z tego
            while (position < code.Length)
            {
                var currentChar = code[position];
                if (this.IsWhiteChar(currentChar))
                {
                    this.position++;
                    continue;
                }

                if (this.IsAnyOfArray(escapeStrings))
                {
                    return finded;
                }

                var oper = TryFindOperator();
                if (oper != null)
                {
                    if (oper.Precedence <= escapePrecedence)
                        return finded;

                    this.position += oper.Coded.Length;
                    var right = readNormal(escapeStrings, oper.Precedence);
                    finded = OperatorExpression.Create(oper, finded, right);
                    continue;
                }

                if (this.IsSpecialChar(currentChar))
                {
                    switch (currentChar)
                    {
                        //case '\'':
                        //    finded = this.findBasicString();
                        //    break;
                        //case '"':
                        //    finded = this.findComplexString();
                        //    break;
                        default:
                            throw new NotImplementedException();
                    }
                }

                if (currentChar >= '0' && currentChar <= '9')
                {
                    finded = this.ReadNumber();
                    continue;
                }
                else
                {
                    var word = this.FindWord();
                    if (keywords.Contains(word.ToLowerInvariant()))
                    {
                        throw new SyntaxError("unexpected keyword");
                    }
                    else
                    {
                        finded = new Identifier(word);
                    }
                }
            }

            return finded;
        }

        private Expressions.Literals.Number ReadNumber(bool allowE = true)
        {
            var number = new Expressions.Literals.Number();
            while (position < code.Length)
            {
                var currentChar = code[position];
                if (currentChar >= '0' && currentChar <= '9')
                {
                    if (number.hasDot)
                        number.Fraction += currentChar;
                    else
                        number.Integer += currentChar;
                    position++;
                }
                else if (currentChar == '.')
                {
                    if (!number.hasDot)
                    {
                        number.hasDot = true;
                        position++;
                    }
                    else
                    {
                        throw new SyntaxError("Multiple dots not allowed");
                    }
                }
                else if (currentChar == 'e' || currentChar == 'E')
                {
                    if (number.Exponent == null && allowE)
                    {
                        position++;
                        number.Exponent = ReadNumber(false);
                    }
                    else
                    {
                        throw new SyntaxError("Exponential not allowed here");
                    }
                }
                else
                {
                    break;
                }
            }

            return number;
        }

        bool IsWhiteChar(char character)
        {
            return new Regex("\\s").IsMatch(character.ToString());
        }

        bool IsSpecialChar(char character)
        {
            return specialChars.Contains(character);
        }

        OperatorDefinition? TryFindOperator()
        {
            foreach (var x in OperatorDefinition.All)
            {
                if (code.Substring(this.position, x.Coded.Length) == x.Coded)
                    return x;
            }

            return null;
        }

        bool IsAnyOfArray(IEnumerable<string>? arr)
        {
            if (arr == null) return false;
            foreach (var x in arr)
            {
                if (this.position + x.Length < this.code.Length && this.code.Substring(this.position, x.Length) == x)
                {
                    return true;
                }
            }

            return false;
        }

        string FindWord()
        {
            string word = "";
            SkipWhiteChars();

            while (this.position < this.code.Length)
            {
                var character = this.code[this.position];
                if (this.IsWhiteChar(character))
                {
                    break;
                }

                if (this.IsSpecialChar(character))
                {
                    break;
                }

                word += character;
                this.position++;
            }

            return word;
        }

        List<string> FindMultword()
        {
            var ret = new List<string>();
            while (this.position < this.code.Length)
            {
                SkipWhiteChars();
                var word = FindWord();
                if (word.Length == 0)
                    throw new SyntaxError("too many dots");
                ret.Add(word);
                SkipWhiteChars();
                if (this.position >= this.code.Length || code[position] != '.')
                    break;
                else
                    position++;
            }

            return ret;
        }

        string TryFindWord()
        {
            string word = "";
            var localPosition = this.position;
            while (true)
            {
                if (this.position == this.code.Length)
                    return "";
                var character = this.code[this.position];
                if (!this.IsWhiteChar(character))
                    break;

                position++;
            }

            while (localPosition < this.code.Length)
            {
                var character = this.code[localPosition];
                if (this.IsWhiteChar(character))
                {
                    break;
                }

                if (this.IsSpecialChar(character))
                {
                    break;
                }

                word += character;
                localPosition++;
            }

            return word;
        }

        void SkipWhiteChars()
        {
            while (position < code.Length)
            {
                var currentChar = code[position];
                if (this.IsWhiteChar(currentChar))
                {
                    this.position++;
                }
                else
                {
                    return;
                }
            }
        }
    }
}