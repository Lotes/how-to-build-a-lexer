using System;
using System.Collections.Generic;
using System.Linq;

namespace Lexer.Automaton.Impl
{
    public class AutomatonBuilder : IAutomatonBuilder
    {
        private int StateCounter = 0;
        private Dictionary<int, Dictionary<char, HashSet<int>>> transitions = new Dictionary<int, Dictionary<char, HashSet<int>>>();
        private int startState = -1;
        private HashSet<int> acceptingStates = new HashSet<int>();
        
        public int AddState()
        {
            return StateCounter++;
        }

        public void AddTransition(int source, char character, int target)
        {
            if(source < 0 || source >= StateCounter) throw new ArgumentException(nameof(source));
            if(target < 0 || target >= StateCounter) throw new ArgumentException(nameof(target));
            transitions.GetValueOrInsertedLazyDefault(source, () => new Dictionary<char, HashSet<int>>())
                .GetValueOrInsertedLazyDefault(character, () => new HashSet<int>())
                .Add(target);
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
                throw new InvalidOperationException("No states defined!");
            if(startState == -1)
                throw new InvalidOperationException("No start defined!");
            return new Automaton(StateCounter, startState, acceptingStates, transitions);
        }
    }
}