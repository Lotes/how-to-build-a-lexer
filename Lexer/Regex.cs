using Lexer.Automaton;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexer
{
    public static class Regex
    {
        private static IRegexParser parser = new RegexParser();
        public static IAutomaton Parse(string input)
        {
            return parser.Parse(input);
        }
    }
}
