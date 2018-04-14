using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml.Schema;
using Lexer.Automaton;

namespace Lexer.RegularExpression.Impl
{
    public enum CharType
    {
        Invalid,
        Special,
        Literal
    }

    public class Parser: IParser
    {
        public static readonly CharSet HexChars = new CharSet(new CharRange('0', '9'), new CharRange('a', 'f'), new CharRange('A', 'Z'));
        public static readonly Dictionary<string, ICharSet> escapesTo = new Dictionary<string, ICharSet>();
        public static readonly Dictionary<char, CharType> asciiTable = new Dictionary<char, CharType>();

        static Parser()
        {
            asciiTable['\u0000'] = CharType.Invalid;
            asciiTable['\u0001'] = CharType.Invalid;
            asciiTable['\u0002'] = CharType.Invalid;
            asciiTable['\u0003'] = CharType.Invalid;
            asciiTable['\u0004'] = CharType.Invalid;
            asciiTable['\u0005'] = CharType.Invalid;
            asciiTable['\u0006'] = CharType.Invalid;
            asciiTable['\u0007'] = CharType.Invalid;
            asciiTable['\u0008'] = CharType.Invalid;
            asciiTable['\u0009'] = CharType.Invalid;
            asciiTable['\u000A'] = CharType.Invalid;
            asciiTable['\u000B'] = CharType.Invalid;
            asciiTable['\u000C'] = CharType.Invalid;
            asciiTable['\u000D'] = CharType.Invalid;
            asciiTable['\u000E'] = CharType.Invalid;
            asciiTable['\u000F'] = CharType.Invalid;

            asciiTable['\u0010'] = CharType.Invalid;
            asciiTable['\u0011'] = CharType.Invalid;
            asciiTable['\u0012'] = CharType.Invalid;
            asciiTable['\u0013'] = CharType.Invalid;
            asciiTable['\u0014'] = CharType.Invalid;
            asciiTable['\u0015'] = CharType.Invalid;
            asciiTable['\u0016'] = CharType.Invalid;
            asciiTable['\u0017'] = CharType.Invalid;
            asciiTable['\u0018'] = CharType.Invalid;
            asciiTable['\u0019'] = CharType.Invalid;
            asciiTable['\u001A'] = CharType.Invalid;
            asciiTable['\u001B'] = CharType.Invalid;
            asciiTable['\u001C'] = CharType.Invalid;
            asciiTable['\u001D'] = CharType.Invalid;
            asciiTable['\u001E'] = CharType.Invalid;
            asciiTable['\u001F'] = CharType.Invalid;

            asciiTable[' '] = CharType.Literal;
            asciiTable['!'] = CharType.Literal;
            asciiTable['"'] = CharType.Special;
            asciiTable['#'] = CharType.Literal;
            asciiTable['$'] = CharType.Special;
            asciiTable['%'] = CharType.Literal;
            asciiTable['&'] = CharType.Literal;
            asciiTable['\''] = CharType.Literal;
            asciiTable['('] = CharType.Special;
            asciiTable[')'] = CharType.Special;
            asciiTable['*'] = CharType.Special;
            asciiTable['+'] = CharType.Special;
            asciiTable[','] = CharType.Literal;
            asciiTable['-'] = CharType.Special;
            asciiTable['.'] = CharType.Special;
            asciiTable['/'] = CharType.Literal;
            asciiTable['0'] = CharType.Literal;
            asciiTable['1'] = CharType.Literal;
            asciiTable['2'] = CharType.Literal;
            asciiTable['3'] = CharType.Literal;
            asciiTable['4'] = CharType.Literal;
            asciiTable['5'] = CharType.Literal;
            asciiTable['6'] = CharType.Literal;
            asciiTable['7'] = CharType.Literal;
            asciiTable['8'] = CharType.Literal;
            asciiTable['9'] = CharType.Literal;
            asciiTable[':'] = CharType.Literal;
            asciiTable[';'] = CharType.Literal;
            asciiTable['<'] = CharType.Literal;
            asciiTable['='] = CharType.Literal;
            asciiTable['>'] = CharType.Literal;
            asciiTable['?'] = CharType.Special;
            asciiTable['@'] = CharType.Literal;
            asciiTable['A'] = CharType.Literal;
            asciiTable['B'] = CharType.Literal;
            asciiTable['C'] = CharType.Literal;
            asciiTable['D'] = CharType.Literal;
            asciiTable['E'] = CharType.Literal;
            asciiTable['F'] = CharType.Literal;
            asciiTable['G'] = CharType.Literal;
            asciiTable['H'] = CharType.Literal;
            asciiTable['I'] = CharType.Literal;
            asciiTable['J'] = CharType.Literal;
            asciiTable['K'] = CharType.Literal;
            asciiTable['L'] = CharType.Literal;
            asciiTable['M'] = CharType.Literal;
            asciiTable['N'] = CharType.Literal;
            asciiTable['O'] = CharType.Literal;
            asciiTable['P'] = CharType.Literal;
            asciiTable['Q'] = CharType.Literal;
            asciiTable['R'] = CharType.Literal;
            asciiTable['S'] = CharType.Literal;
            asciiTable['T'] = CharType.Literal;
            asciiTable['U'] = CharType.Literal;
            asciiTable['V'] = CharType.Literal;
            asciiTable['W'] = CharType.Literal;
            asciiTable['X'] = CharType.Literal;
            asciiTable['Y'] = CharType.Literal;
            asciiTable['Z'] = CharType.Literal;
            asciiTable['['] = CharType.Special;
            asciiTable['\\'] = CharType.Special;
            asciiTable[']'] = CharType.Special;
            asciiTable['^'] = CharType.Special;
            asciiTable['_'] = CharType.Literal;
            asciiTable['`'] = CharType.Literal;
            asciiTable['a'] = CharType.Literal;
            asciiTable['b'] = CharType.Literal;
            asciiTable['c'] = CharType.Literal;
            asciiTable['d'] = CharType.Literal;
            asciiTable['e'] = CharType.Literal;
            asciiTable['f'] = CharType.Literal;
            asciiTable['g'] = CharType.Literal;
            asciiTable['h'] = CharType.Literal;
            asciiTable['i'] = CharType.Literal;
            asciiTable['j'] = CharType.Literal;
            asciiTable['k'] = CharType.Literal;
            asciiTable['l'] = CharType.Literal;
            asciiTable['m'] = CharType.Literal;
            asciiTable['n'] = CharType.Literal;
            asciiTable['o'] = CharType.Literal;
            asciiTable['p'] = CharType.Literal;
            asciiTable['q'] = CharType.Literal;
            asciiTable['r'] = CharType.Literal;
            asciiTable['s'] = CharType.Literal;
            asciiTable['t'] = CharType.Literal;
            asciiTable['u'] = CharType.Literal;
            asciiTable['v'] = CharType.Literal;
            asciiTable['w'] = CharType.Literal;
            asciiTable['x'] = CharType.Literal;
            asciiTable['y'] = CharType.Literal;
            asciiTable['z'] = CharType.Literal;
            asciiTable['{'] = CharType.Special;
            asciiTable['|'] = CharType.Special;
            asciiTable['}'] = CharType.Special;
            asciiTable['~'] = CharType.Literal;
            asciiTable['\u007F'] = CharType.Invalid;

            foreach (var entry in asciiTable)
                if (entry.Value == CharType.Special)
                    escapesTo[entry.Key.ToString()] = new CharSet(entry.Key);

            escapesTo["."] = CharSet.Full;
            escapesTo["d"] = new CharSet(new CharRange('0', '9'));
            escapesTo["n"] = new CharSet('\n');
            escapesTo["r"] = new CharSet('\r');
            escapesTo["s"] = new CharSet(' ', '\t', '\r', '\n');
            escapesTo["t"] = new CharSet('\t');
        }

        public IAutomaton Parse(string input)
        {
            /* Grammar:
             * S = X '|' 'S'
             *   / X 
             * X = YX
             *   / Y
             * Y = P
             *   / P '*'
             *   / P '+'
             *   / P '?'
             *   / P '{' N ',' N '}'
             *   / '(' S ')'
             * P = '[' '^'? R+ ']'
             *   / K
             * N = [0-9]+
             */
            return new TopDownParser(input).Exec().Determinize().Minimize();
        }

        private class TopDownParser
        {
            private const char Eos = '\0';
            private string input;
            private int index;
            
            public TopDownParser(string input)
            {
                this.input = input;
                this.index = 0;
            }
            
            public char Lookahead
            {
                get { return index >= input.Length ? Eos : input[index]; }
            }
            
            public IAutomaton Exec()
            {
                var lhs = ConcatExpression();
                while (Lookahead == '|')
                {
                    index++;
                    var rhs = ConcatExpression();
                    lhs = ConstructionKit.Alternate(new[]
                    {
                        lhs, rhs
                    });
                }
                return lhs;
            }

            private IAutomaton ConcatExpression()
            {
                var lhs = PrimaryExpression();
                while (Lookahead != '|' && Lookahead != Eos && Lookahead != ')')
                {
                    var rhs = PrimaryExpression();
                    lhs = ConstructionKit.Concat(new[]
                    {
                        lhs, rhs
                    });
                }
                return lhs;
            }

            private IAutomaton PrimaryExpression()
            {
                var operand = Factor();
                switch (Lookahead)
                {
                    case '*': index++; return ConstructionKit.Repeat(operand);
                    case '+': index++; return ConstructionKit.Repeat(operand, 1);
                    case '?': index++; return ConstructionKit.Repeat(operand, 0, 1);
                    case '{':
                        index++;
                        var min = Number();
                        if (MayConsume(','))
                        {
                            var max = Number();
                            Consumes('}');
                            return ConstructionKit.Repeat(operand, min, max);
                        }
                        else
                        {
                            Consumes('}');
                            return ConstructionKit.Repeat(operand, min, min);
                        }
                }
                return operand;
            }

            private int Number()
            {
                if(Lookahead < '0' || Lookahead > '9')
                    throw new InvalidOperationException("Digit expected, but '"+Lookahead+"' found!");
                var num = "";
                while (Lookahead >= '0' && Lookahead <= '9')
                {
                    num += Lookahead;
                    index++;
                }
                return Convert.ToInt32(num);
            }

            private bool MayConsume(char c)
            {
                if (Lookahead != c) return false;
                index++;
                return true;
            }

            private bool MayConsume(CharSet c)
            {
                if (!c.Includes(Lookahead)) return false;
                index++;
                return true;
            }

            private bool MayConsume(string str)
            {
                var lookaheadString = input.Substring(index, str.Length);
                if (lookaheadString != str) return false;
                index += str.Length;
                return true;
            }

            private void Consumes(char c)
            {
                if(Lookahead != c)
                    throw new InvalidOperationException("'"+c+"' expected, but '"+Lookahead+"' found.");
                index++;
            }

            private IAutomaton Factor()
            {
                if (MayConsume('('))
                {
                    var result = Exec();
                    Consumes(')');
                    return result;
                }
                else if(MayConsume('['))
                {
                    var negate = MayConsume('^');
                    var chars = CharSet.Empty;
                    while (!MayConsume(']'))
                    {
                        var set = CharRange();
                        chars = chars.Union(set);
                    }
                    if (negate)
                        chars = chars.Negate();
                    return ConstructionKit.Consume(chars);
                }
                else
                {
                    var ch = CharRange();
                    return ConstructionKit.Consume(ch);
                }
            }

            private ICharSet CharRange()
            {
                ICharSet result;
                var first = Char();
                if(first.Length == 1 && MayConsume('-'))
                {
                    var last = Char();
                    if (last.Length != 1)
                        throw new InvalidOperationException("Invalid upper bound for character range!");
                    result = new CharSet(new CharRange(first.First().First(), last.First().First()));
                }
                else
                {
                    result = first;
                }
                return result;
            }

            private ICharSet Char()
            {
                var result = CharSet.Empty;
                if(asciiTable.ContainsKey(Lookahead))
                {
                    switch(asciiTable[Lookahead])
                    {
                        case CharType.Invalid:
                            throw new InvalidOperationException("Invalid character!");
                        case CharType.Literal:
                            var first = Lookahead;
                            index++;
                            result = result.Union(new CharSet(first));
                            break;
                        case CharType.Special:
                            if (MayConsume('.'))
                            {
                                result = CharSet.Full;
                            }
                            else if (MayConsume('\\'))
                            {
                                if (MayConsume('u'))
                                {
                                    char digit;
                                    int value = 0;
                                    while ((digit = Lookahead) > 0 && MayConsume(HexChars))
                                        value = 16 * value + int.Parse(digit.ToString(), NumberStyles.HexNumber);
                                    result = result.Union(new CharSet((char)value));
                                }
                                else
                                {
                                    var found = false;
                                    foreach (var kv in escapesTo)
                                    {
                                        if (MayConsume(kv.Key))
                                        {
                                            result = result.Union(kv.Value);
                                            found = true;
                                            break;
                                        }
                                    }
                                    if (!found)
                                        throw new InvalidOperationException("Invalid escape character: " + Lookahead);
                                }
                            }
                            else
                                throw new InvalidOperationException("Invalid character!");
                            break;
                    }
                }
                return result;
            }
        }
    }
}