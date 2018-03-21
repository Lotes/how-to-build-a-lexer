using System;
using System.Collections.Generic;
using System.Data;
using Lexer.Automaton.Impl;

namespace Lexer.Automaton
{
    public static class AutomatonConstructionKit
    {
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
            var previousStart = -1;
            var start = builder.AddState();
            var accept = builder.AddState();
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