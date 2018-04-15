using System;
using System.Collections.Generic;

namespace Lexer
{
    public static class LexerExtensions
    {
        public static IEnumerable<IToken> Read(this ILexer lexer, string input)
        {
            var index = 0;
            IToken token;
            while (index < input.Length && lexer.Read(input, index, out token))
            {
                yield return token;
                index += token.Value.Length;
            }
            if (index < input.Length)
                throw new InvalidOperationException("EOF not reached!");
        }
    }
}
