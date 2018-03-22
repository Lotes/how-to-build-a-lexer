using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using Lexer.Automaton.Impl;

namespace Lexer.Automaton
{
    public static class AutomatonConstructionKit
    {
        public static IAutomaton Optimize(this IAutomaton @this)
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
                    foreach (var c in transition.Targets.Keys)
                        if(c != '\0')
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
                        var targets = transition.Targets.GetOrDefault(c);
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