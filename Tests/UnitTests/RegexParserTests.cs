using Lexer;
using Lexer.Automaton;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.UnitTests
{
    [TestClass]
    public class RegexParserTests
    {
        private IAutomaton ParseRegex(string input)
        {
            return Regex.Parse(input);
        }

        [TestMethod]
        public void ParseConcatenation()
        {
            var automaton = ParseRegex("ab");
            automaton.Accepts("ab");
            automaton.Rejects("a");
            automaton.Rejects("b");
            automaton.Rejects("ba");
        }

        [TestMethod]
        public void ParseStarRepetition()
        {
            var automaton = ParseRegex("a*");
            automaton.Accepts("");
            automaton.Accepts("a");
            automaton.Accepts("aa");
            automaton.Accepts("aaaaaa");
            automaton.Rejects("b");
        }

        [TestMethod]
        public void ParsePlusRepetition()
        {
            var automaton = ParseRegex("a+");
            automaton.Rejects("");
            automaton.Accepts("a");
            automaton.Accepts("aa");
            automaton.Accepts("aaaaaa");
            automaton.Rejects("b");
        }

        [TestMethod]
        public void ParseMaybeOperator()
        {
            var automaton = ParseRegex("a?");
            automaton.Accepts("");
            automaton.Accepts("a");
            automaton.Rejects("aa");
        }

        [TestMethod]
        public void ParseFixedRepetition()
        {
            var automaton = ParseRegex("a{3}");
            automaton.Accepts("aaa");
            automaton.Rejects("aa");
            automaton.Rejects("aaaa");
        }

        [TestMethod]
        public void ParseRangeRepetition()
        {
            var automaton = ParseRegex("a{3,6}");
            automaton.Accepts("aaa");
            automaton.Accepts("aaaa");
            automaton.Accepts("aaaaaa");
            automaton.Rejects("aaaaaaa");
            automaton.Rejects("aa");
        }

        [TestMethod]
        public void ParseHexChar()
        {
            var automaton = ParseRegex("\\u20");
            automaton.Accepts(" ");
            automaton.Rejects("\t");
        }

        [TestMethod]
        public void ParseEscapedChar()
        {
            var automaton = ParseRegex("\\n");
            automaton.Accepts("\n");
            automaton.Rejects("\r");
        }

        [TestMethod]
        public void ParseAnyChar()
        {
            var automaton = ParseRegex(".");
            automaton.Accepts("a");
            automaton.Accepts("z");
            automaton.Accepts(".");
            automaton = ParseRegex("\\.");
            automaton.Rejects("a");
            automaton.Rejects("z");
            automaton.Accepts(".");
        }
    }
}
