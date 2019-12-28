using SQLProto.Parser.Expressions;
using SQLProto.Parser.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SQLProto.Parser
{
    public class Parser
    {
        private string code;
        private int position;
        private static string[] keywords = { "as" };
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

                        default:
                            throw new SyntaxError("unexpected keyword");
                    }
                }
            }
            return finded;
        }
        Select ReadSelect(IEnumerable<string> escapeStrings = null)
        {
            var query = new Select();
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
                    return query;
                }
                else
                {
                    var itemStart = position;
                    var item = readNormal(new string[] { ",", "as" });
                    if (item != null)
                    {
                        var name = code.Substring(itemStart, position - itemStart);

                        query.Selects.Add(new Expressions.Literals.NamedExpression(name, item));
                    }
                }
            }
            return query;
        }

        IExpression readNormal(IEnumerable<string> escapeStrings = null)
        {
            IExpression finded = null;//todo zrobić stack z tego
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
                else
                {
                    if (currentChar >= '0' && currentChar <= '9')
                    {
                        finded = this.ReadNumber();
                    }
                    else
                    {
                        var word = this.FindWord();
                        if (keywords.Contains(word.ToLowerInvariant()))
                            switch (word.ToLower())
                            {


                                default:
                                    throw new SyntaxError("unexpected keyword");
                            }

                        else
                        {
                            //finded = 
                        }
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
            return false;//todo
            //return Syntax.SpecialChars.includes(char);
        }
        bool IsAnyOfArray(IEnumerable<string> arr)
        {
            if (arr == null) return false;
            foreach (var x in arr)
            {
                if (this.code.Substring(this.position, x.Length) == x)
                {
                    return true;
                }
            }
            return false;
        }
        string FindWord()
        {
            string word = "";
            while (true)
            {
                if (this.position == this.code.Length)
                    throw new SyntaxError("Unexpected end of file");
                var character = this.code[this.position];
                if (!this.IsWhiteChar(character))
                    break;

                this.position++;
            }
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
    }
}
