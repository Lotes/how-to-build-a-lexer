using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexer.Automaton
{
    public class AutomatonConstructionException : Exception
    {
        public AutomatonConstructionException(string message) : base(message)
        {
        }
    }
}
