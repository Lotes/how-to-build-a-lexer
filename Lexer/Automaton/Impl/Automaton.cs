using System;
using System.Collections.Generic;
using System.Linq;

namespace Lexer.Automaton.Impl
{
    public class Automaton: IAutomaton
    {
        public Automaton(int stateCount, int startState, IEnumerable<int> acceptingStates,
            Dictionary<int, ITransitionTargets> transitions)
        {
            StateCount = stateCount;
            StartState = startState;
            AcceptingStates = new HashSet<int>(acceptingStates);
            TransitionsBySource = transitions;
        }
        public int StartState { get; }
        public int StateCount { get; }
        public ISet<int> AcceptingStates { get; }
        public IReadOnlyDictionary<int, ITransitionTargets> TransitionsBySource { get; private set; }
    }
}