using System.Collections.Generic;

namespace Lexer.Automaton
{
    public interface IAutomaton
    {
        int StartState { get; }
        int StateCount { get; }
        ISet<int> AcceptingStates { get; }
        IReadOnlyDictionary<int, IReadOnlyDictionary<char, ISet<int>>> TransitionsBySource { get; }
    }
}