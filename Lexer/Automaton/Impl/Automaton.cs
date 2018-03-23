using System;
using System.Collections.Generic;
using System.Linq;

namespace Lexer.Automaton.Impl
{
    public class Automaton: IAutomaton
    {
        public Automaton(int stateCount, int startState, IEnumerable<int> acceptingStates,
            Dictionary<int, Dictionary<char, HashSet<int>>> transitions)
        {
            StateCount = stateCount;
            StartState = startState;
            AcceptingStates = new HashSet<int>(acceptingStates);
            TransitionsBySource = transitions.ToDictionary(t => t.Key, t => (IReadOnlyDictionary<char, ISet<int>>) t.Value.ToDictionary(kv => kv.Key, kv => (ISet<int>)kv.Value));
        }
        public int StartState { get; }
        public int StateCount { get; }
        public ISet<int> AcceptingStates { get; }
        public IReadOnlyDictionary<int, IReadOnlyDictionary<char, ISet<int>>> TransitionsBySource { get; }
    }
}