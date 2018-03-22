using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Xml.Schema;
using Lexer.Automaton;

namespace Lexer.RegularExpression.Impl
{
    public class Parser
    {
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
            return new TopDownParser(input).Exec();
        }

        private class TopDownParser
        {
            private string input;
            private int index;
            
            public TopDownParser(string input)
            {
                this.input = input;
                this.index = 0;
            }
            
            public char Lookahead
            {
                get { return index >= input.Length ? '\0' : input[index]; }
            }
            
            public IAutomaton Exec()
            {
                var lhs = ConcatExpression();
                while (Lookahead == '|')
                {
                    index++;
                    var rhs = ConcatExpression();
                    lhs = AutomatonConstructionKit.Alternate(new[]
                    {
                        lhs, rhs
                    });
                }
                return lhs;
            }

            private IAutomaton ConcatExpression()
            {
                var lhs = PrimaryExpression();
                while (Lookahead != '|' && Lookahead != '\0' && Lookahead != ')')
                {
                    var rhs = PrimaryExpression();
                    lhs = AutomatonConstructionKit.Concat(new[]
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
                    case '*': index++; return AutomatonConstructionKit.Repeat(operand);
                    case '+': index++; return AutomatonConstructionKit.Repeat(operand, 1);
                    case '?': index++; return AutomatonConstructionKit.Repeat(operand, 0, 1);
                    case '{':
                        index++;
                        var min = Number();
                        if (MayConsume(','))
                        {
                            var max = Number();
                            Consumes('}');
                            return AutomatonConstructionKit.Repeat(operand, min, max);
                        }
                        else
                        {
                            Consumes('}');
                            return AutomatonConstructionKit.Repeat(operand, min, min);
                        }
                        break;
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
                else
                {
                    var result = AutomatonConstructionKit.Consume(new[]{ Lookahead });
                    index++;
                    return result;   
                }
            }
        }
    }
}