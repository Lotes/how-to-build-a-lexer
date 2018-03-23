using System.Collections.Generic;
using System.Linq;

namespace Lexer.Automaton
{
    public static class AutomatonBuilderExtensions
    {
        public static void AddEpsilonTransition(this IAutomatonBuilder @this, int source, int target)
        {
            @this.AddTransition(source, AutomatonExtensions.Epsilon, target);
        }

        public static void CopyTo(this IAutomaton @this, IAutomatonBuilder builder, out int oldNewStartState,
            out IEnumerable<int> oldNewAcceptingStates)
        {
            var stateMapping = new Dictionary<int, int>();
            for (var state = 0; state < @this.StateCount; state++)
                stateMapping.Add(state, builder.AddState());
            foreach (var kv in @this.TransitionsBySource)
            {
                var source = stateMapping[kv.Key];
                var transition = kv.Value;
                foreach (var target in transition)
                {
                    foreach(var x in target.Value.Select(v => stateMapping[v]))
                        builder.AddTransition(source, target.Key, x);
                }
            }
            oldNewStartState = stateMapping[@this.StartState];
            oldNewAcceptingStates = @this.AcceptingStates.Select(s => stateMapping[s]).ToArray();
        }
    }
}