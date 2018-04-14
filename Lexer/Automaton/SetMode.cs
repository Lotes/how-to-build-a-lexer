using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexer.Automaton
{
    public enum SetMode
    {
        Included,
        Excluded
    }

    public static class SetModeExtensions
    {
        public static SetMode Negate(this SetMode @this)
        {
            return @this == SetMode.Excluded ? SetMode.Included : SetMode.Excluded;
        }
    }
}
