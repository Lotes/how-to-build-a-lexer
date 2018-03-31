using Lexer.Automaton;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.UnitTests
{
    public static class AutomatonTestExtensions
    {
        public static void Accepts(this IAutomaton @this, string input)
        {
            Assert.IsTrue(@this.Read(input));
        }

        public static void Rejects(this IAutomaton @this, string input)
        {
            Assert.IsFalse(@this.Read(input));
        }
    }
}
