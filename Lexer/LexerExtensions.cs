using System;
using System.Collections.Generic;
using System.Text;

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
                throw new LexerExecutionException("EOF not reached!");
        }

        public static string Times(this string str, int factor)
        {
            if (factor < 0)
                throw new ArgumentException("Factor must be non-negative!", nameof(factor));
            var builder = new StringBuilder(str.Length*factor);
            while(factor > 0)
            {
                builder.Append(str);
                factor--;
            }
            return builder.ToString();
        }
    }
}
