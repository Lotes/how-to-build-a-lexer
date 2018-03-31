using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexer.Automaton
{
    public static class CharSetExtensions
    {
        public static void Add(this CharSet @this, CharSet other)
        {
            foreach (var range in other)
                @this.Add(range.From, range.To);
        }
    }
}
