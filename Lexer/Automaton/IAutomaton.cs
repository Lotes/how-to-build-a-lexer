using System.Collections.Generic;
using System.Linq;

namespace Lexer.Automaton
{
    public interface IAutomaton
    {
        int StartState { get; }
        int StateCount { get; }
        ISet<int> AcceptingStates { get; }
        IReadOnlyDictionary<int, ILookup<char?, int>> TransitionsBySource { get; }
    }
}