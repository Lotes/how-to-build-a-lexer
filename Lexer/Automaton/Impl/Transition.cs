using System.Collections.Generic;
using System.Linq;

namespace Lexer.Automaton.Impl
{
    public class Transition : ITransition
    {
        public Transition(Dictionary<char, HashSet<int>> argValue)
        {
            Targets = argValue.ToDictionary(kv => kv.Key, kv => (IEnumerable<int>)kv.Value);
        }

        public IReadOnlyDictionary<char, IEnumerable<int>> Targets { get; }
    }
}