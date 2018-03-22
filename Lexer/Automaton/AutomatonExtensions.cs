using System;
using System.Collections.Generic;
using System.Linq;

namespace Lexer.Automaton
{
    public static class AutomatonExtensions
    {
        public static void Print(this IAutomaton @this)
        {
            Console.WriteLine($"start: {@this.StartState}");
            Console.WriteLine($"ends: {string.Join(",", @this.AcceptingStates)}");
            foreach (var kv in @this.TransitionsBySource)
            {
                var source = kv.Key;
                foreach (var t in kv.Value.Targets)
                {
                    Console.WriteLine($"{source} --{t.Key}--> {string.Join(",", t.Value)}");
                }
            }
        }

        public static ISet<int> GetEpsilonClosure(this IAutomaton @this, params int[] states)
        {
            var set = new HashSet<int>();
            var queue = new Queue<int>();
            foreach(var state in states)
                queue.Enqueue(state);
            while (queue.Any())
            {
                var source = queue.Dequeue();
                set.Add(source);
                var targets = @this.TransitionsBySource.GetOrDefault(source)?.Targets
                    .GetOrDefault('\0') ?? Enumerable.Empty<int>();
                foreach(var target in targets)
                    if(!set.Contains(target))
                        queue.Enqueue(target);
            }
            return set;
        }

        public static decimal ToBits(this IEnumerable<int> @this)
        {
            var result = 0m;
            foreach (var state in @this)
            {
                var shiftCount = (decimal)state;
                var bit = 1;
                while (shiftCount > 0)
                {
                    bit *= 2;
                    shiftCount--;
                }
                result += bit;
            }
            return result;
        }
    }
}