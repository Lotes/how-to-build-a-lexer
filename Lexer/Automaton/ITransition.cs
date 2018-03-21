using System.Collections.Generic;

namespace Lexer.Automaton
{
    public interface ITransition
    {
        /// <summary>
        /// char \0 is epsilon
        /// </summary>
        IReadOnlyDictionary<char, IEnumerable<int>> Targets {get;}
    }
}