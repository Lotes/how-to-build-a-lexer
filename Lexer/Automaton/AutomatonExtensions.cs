using Lexer.Automaton.Impl;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lexer.Automaton
{
    public static class AutomatonExtensions
    {
        public static ITransitionTargets EmptyTargets = new TransitionTargets();
        private static ISet<int> EmptyStates = new HashSet<int>();
        public static bool Read(this IAutomaton @this, string input)
        {
            @this.Print();
            var state = new HashSet<int>(@this.GetEpsilonClosure(@this.StartState));
            foreach(var c in input)
            {
                state = new HashSet<int>(
                    @this.GetEpsilonClosure(
                        state.SelectMany(s => @this.TransitionsBySource
                            .GetOrDefault(s, EmptyTargets)
                            .ReadChar(c))
                            .ToArray()));
                if (!state.Any())
                    return false;
            }
            return @this.AcceptingStates.Intersect(state).Any();
        }

        public static void Print(this IAutomaton @this)
        {
            Console.WriteLine($"start: {@this.StartState}");
            Console.WriteLine($"ends: {string.Join(",", @this.AcceptingStates)}");
            foreach (var kv in @this.TransitionsBySource)
            {
                var source = kv.Key;
                foreach (var t in kv.Value)
                {
                    Console.WriteLine($"{source} --{t.Key}--> {string.Join(",", t)}");
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
                var targets = @this.TransitionsBySource.GetOrDefault(source)?
                    [CharSet.Epsilon] ?? Enumerable.Empty<int>();
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