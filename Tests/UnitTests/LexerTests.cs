using Lexer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.UnitTests
{
    [TestClass]
    public class LexerTests
    {
        [TestMethod]
        public void TestLexer()
        {
            var typeNumbers = new TokenType("NUMBER", Regex.Parse("\\d+"), 1);
            var typeId = new TokenType("ID", Regex.Parse("[a-zA-Z_][a-zA-Z0-9_]*"), 1);
            var typeWhitespace = new TokenType("SPACE", Regex.Parse("\\s"), 1);
            var lexer = new Lexer.Lexer(new[] 
            {
                typeNumbers,
                typeId,
                typeWhitespace
            });
            foreach(var token in lexer.Read("Hallo Du 3"))
            {
                Console.WriteLine(token);
            }
        }
    }
}
