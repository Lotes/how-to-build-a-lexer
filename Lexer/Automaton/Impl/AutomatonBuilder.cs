using System;
using System.Collections.Generic;
using System.Linq;

namespace Lexer.Automaton.Impl
{
    public class AutomatonBuilder : IAutomatonBuilder
    {
        private int StateCounter = 0;
        private Dictionary<int, TransitionTargets> transitions = new Dictionary<int, TransitionTargets>();
        private int startState = -1;
        private HashSet<int> acceptingStates = new HashSet<int>();
        
        public int AddState()
        {
            return StateCounter++;
        }

        public void AddTransition(int source, ICharSet characters, int target)
        {
            if(source < 0 || source >= StateCounter) throw new ArgumentException(nameof(source));
            if(target < 0 || target >= StateCounter) throw new ArgumentException(nameof(target));
            transitions.GetValueOrInsertedLazyDefault(source, () => new TransitionTargets())
                .Add(characters, target);
        }

        public void SetStartState(int state)
        {
            if(state < 0 || state >= StateCounter) throw new ArgumentException(nameof(state));
            startState = state;
        }

        public void AcceptState(int state)
        {
            if(state < 0 || state >= StateCounter) throw new ArgumentException(nameof(state));
            acceptingStates.Add(state);
        }

        public IAutomaton Build()
        {
            if(StateCounter == 0)
                throw new AutomatonConstructionException("No states defined!");
            if(startState == -1)
                throw new AutomatonConstructionException("No start defined!");
            return new Automaton(StateCounter, startState, acceptingStates, 
                transitions.ToDictionary(kv => kv.Key, kv => (ITransitionTargets)kv.Value));
        }
    }
}