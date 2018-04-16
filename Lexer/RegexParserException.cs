using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexer
{
    public class RegexParserException : Exception
    {
        public RegexParserException(int index, string message) : base(message)
        {
            Index = index;
        }
        public int Index { get; }
    }
}
