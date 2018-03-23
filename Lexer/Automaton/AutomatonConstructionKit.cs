﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using Lexer.Automaton.Impl;

namespace Lexer.Automaton
{
    public static class AutomatonConstructionKit
    {
        private static readonly Dictionary<char, ISet<int>> Empty = new Dictionary<char, ISet<int>>();
        public static IAutomaton Minimize(this IAutomaton @this)
        {
            //prepare NFA automaton to a DFA automaton
            var automaton = @this.Determinize().CleanUpUnreachables();
            //mark all distinguishable pairs (unacceptable <-> acceptable)
            var stateCount = automaton.StateCount;
            var distinguishablePairs = new bool?[stateCount, stateCount];
            foreach(var acceptable in automaton.AcceptingStates)
            foreach (var unacceptable in Enumerable.Range(0, stateCount)
                .Except(automaton.AcceptingStates))
            {
                distinguishablePairs[acceptable, unacceptable] = true;
                distinguishablePairs[unacceptable, acceptable] = true;
            }
            //try to mark all others
            // Example (#=4)
            // 1 X
            // 2 X X
            // 3 X X X
            //   0 1 2
            var mergedByState = new Dictionary<int, HashSet<int>>();
            var merged = new HashSet<HashSet<int>>();
            for(var aIndex=1; aIndex<stateCount; aIndex++)
            for(var bIndex=0; bIndex<aIndex; bIndex++)
                if (!MarkDistinguishables(distinguishablePairs, automaton, aIndex, bIndex))
                {
                    var list = mergedByState.GetValueOrInsertedDefault(aIndex, mergedByState.GetValueOrInsertedLazyDefault(bIndex, () => new HashSet<int>()));
                    list.Add(aIndex);
                    list.Add(bIndex);
                    merged.Add(list);
                }
            var mappings = new Dictionary<int, int>(); //old automaton to new automaton states
            var inverseMappings = new Dictionary<int, int>();
            //NOW BUILD THE FUCKING AUTOMATON!!!
            var builder = new AutomatonBuilder();
            foreach (var mergedStates in merged)
            {
                var newState = builder.AddState();
                inverseMappings[newState] = mergedStates.First();
                foreach (var oldState in mergedStates)
                    mappings[oldState] = newState;
            }
            for (var state = 0; state < automaton.StateCount; state++)
                if(!mappings.ContainsKey(state))
                {
                    var newState = builder.AddState();
                    mappings[state] = newState;
                    inverseMappings[newState] = state;
                }
            //mark starts and ends
            builder.SetStartState(mappings[automaton.StartState]);
            foreach(var accept in automaton.AcceptingStates)
                builder.AcceptState(mappings[accept]);
            //add transitions
            foreach(var kv in inverseMappings)
            {
                var oldState = kv.Value;
                var newState = kv.Key;
                foreach (var transition in automaton.TransitionsBySource.GetOrDefault(oldState, Empty))
                    builder.AddTransition(newState, transition.Key, mappings[transition.Value.First()]);
            }
            //build
            return builder.Build();
        }

        private static bool MarkDistinguishables(bool?[,] pairs, IAutomaton automaton, int aIndex, int bIndex)
        {
            if (pairs[aIndex, bIndex] != null)
                return pairs[aIndex, bIndex].Value;
            var stateCount = automaton.StateCount;
            var aInputs = automaton.TransitionsBySource.GetOrDefault(aIndex, Empty).Keys;
            var bInputs = automaton.TransitionsBySource.GetOrDefault(bIndex, Empty).Keys;
            bool result;
            if (aInputs.Except(bInputs).Any() || bInputs.Except(aInputs).Any())
                result = true;
            else
            {
                result = false;
                pairs[aIndex, bIndex] = result;
                pairs[bIndex, aIndex] = result;
                foreach (var c in aInputs)
                {
                    var nextAIndex = automaton.TransitionsBySource[aIndex][c].First();
                    var nextBIndex = automaton.TransitionsBySource[bIndex][c].First(); //first, because DFA
                    if (MarkDistinguishables(pairs, automaton, nextAIndex, nextBIndex))
                    {
                        result = true;
                        break;
                    }
                }
            }
            pairs[aIndex, bIndex] = result;
            pairs[bIndex, aIndex] = result;
            return result;
        }

        public static IAutomaton CleanUpUnreachables(this IAutomaton @this)
        {
            return @this;
        }
        public static IAutomaton Determinize(this IAutomaton @this)
        {
            var builder = new AutomatonBuilder();
            var newStates = new Dictionary<decimal, int>();
            var start = @this.GetEpsilonClosure(@this.StartState);
            var queue = new Queue<IEnumerable<int>>();
            var transitions = new LinkedList<Tuple<decimal, char, decimal>>();
            queue.Enqueue(start);
            while (queue.Any())
            {
                var source = queue.Dequeue();
                var sourceBits = source.ToBits();
                if (newStates.ContainsKey(sourceBits))
                    continue;
                var state = builder.AddState();
                newStates[sourceBits] = state;
                
                //define start and end
                if(source == start)
                    builder.SetStartState(state);
                if(source.Any(st => @this.AcceptingStates.Contains(st)))
                    builder.AcceptState(state);
                
                //get all possible inputs
                var inputs = new HashSet<char>();
                foreach (var st in source)
                {
                    var transition = @this.TransitionsBySource.GetOrDefault(st);
                    if (transition == null)
                        continue;
                    foreach (var c in transition.Keys)
                        if(c != AutomatonExtensions.Epsilon)
                            inputs.Add(c);
                }
                
                //find target states (in epsilon closure)
                foreach (var c in inputs)
                {
                    var dfaState = new HashSet<int>();
                    foreach (var st in source)
                    {
                        var transition = @this.TransitionsBySource.GetOrDefault(st);
                        if (transition == null)
                            continue;
                        var targets = transition.GetOrDefault(c);
                        if (targets == null)
                            continue;
                        foreach (var target in targets)
                            dfaState.Add(target);
                    }
                    if (dfaState.Any())
                    {
                        var epsilonTarget = @this.GetEpsilonClosure(dfaState.ToArray());
                        var targetBits = epsilonTarget.ToBits();
                        transitions.AddLast(new Tuple<decimal, char, decimal>(sourceBits, c, targetBits));
                        queue.Enqueue(epsilonTarget);
                    }
                }
            }
            //add all transitions
            foreach (var transition in transitions)
            {
                var source = newStates[transition.Item1];
                var target = newStates[transition.Item3];
                builder.AddTransition(source, transition.Item2, target);
            }
            return builder.Build();
        }
        public static IAutomaton Consume(IEnumerable<char> characters)
        {
            var builder = new AutomatonBuilder();
            var start = builder.AddState();
            var end = builder.AddState();
            builder.SetStartState(start);
            builder.AcceptState(end);
            foreach(var character in characters)
                builder.AddTransition(start, character, end);
            return builder.Build();
        }
        public static IAutomaton Alternate(IEnumerable<IAutomaton> automata)
        {
            var builder = new AutomatonBuilder();
            var start = builder.AddState();
            var end = builder.AddState();
            builder.SetStartState(start);
            builder.AcceptState(end);
            foreach (var automaton in automata)
            {
                automaton.CopyTo(builder, out var oldStart, out var oldAcceptings);
                builder.AddEpsilonTransition(start, oldStart);
                foreach(var accepting in oldAcceptings)
                    builder.AddEpsilonTransition(accepting, end);
            }
            return builder.Build();
        }
        public static IAutomaton Concat(IEnumerable<IAutomaton> automata)
        {
            var builder = new AutomatonBuilder();
            var start = builder.AddState();
            builder.SetStartState(start);
            foreach (var automaton in automata)
            {
                var end = builder.AddState();
                automaton.CopyTo(builder, out var oldNewStart, out var oldNewAcceptings);
                builder.AddEpsilonTransition(start, oldNewStart);
                foreach(var st in oldNewAcceptings)
                    builder.AddEpsilonTransition(st, end);
                start = end;
            }
            builder.AcceptState(start);
            return builder.Build();
        }
        public static IAutomaton Repeat(IAutomaton automaton, int min = 0, int max = -1)
        {
            if(min < 0 || (max >= 0 && min > max))
                throw new ArgumentException(nameof(min));
            var builder = new AutomatonBuilder();
            var start = builder.AddState();
            var accept = builder.AddState();
            var previousStart = -1;
            builder.SetStartState(start);
            builder.AcceptState(accept);
            int? count = null;
            if (max != -1)
                count = max - min;
            while (min > 0)
            {
                var end = builder.AddState();
                automaton.CopyTo(builder, out var oldNewStart, out var oldNewAcceptings);
                builder.AddEpsilonTransition(start, oldNewStart);
                foreach(var st in oldNewAcceptings)
                    builder.AddEpsilonTransition(st, end);
                min--;
                previousStart = start;
                start = end;
            }
            builder.AddEpsilonTransition(start, accept);
            if (count == null) //no upper limit, loop!
            {
                if(previousStart == -1)
                {
                    var end = builder.AddState();
                    automaton.CopyTo(builder, out var oldNewStart, out var oldNewAcceptings);
                    builder.AddEpsilonTransition(start, oldNewStart);
                    builder.AddEpsilonTransition(end, accept);
                    foreach(var st in oldNewAcceptings)
                        builder.AddEpsilonTransition(st, end);
                    previousStart = start;
                    start = end;
                }
                builder.AddEpsilonTransition(start, previousStart);
            }
            else while(count > 0)//existing upper limit, no loop!
            {
                var end = builder.AddState();
                automaton.CopyTo(builder, out var oldNewStart, out var oldNewAcceptings);
                builder.AddEpsilonTransition(start, oldNewStart);
                foreach(var st in oldNewAcceptings)
                    builder.AddEpsilonTransition(st, end);
                builder.AddEpsilonTransition(end, accept);
                count--;
                start = end;
            }
            return builder.Build();
        }
    }
}