using System;
using System.Runtime.Serialization;

namespace Lexer
{
    public class LexerExecutionException : Exception
    {
        public LexerExecutionException(string message) : base(message)
        {
        }
    }
}