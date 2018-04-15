using Lexer.Automaton;
using System.Collections.Generic;
using System.Linq;

namespace Lexer
{
    public class Lexer : ILexer
    {
        private ITokenType[] types;
        public Lexer(IEnumerable<ITokenType> types)
        {
            this.types = types.ToArray();
        }
        public bool Read(string input, int index, out IToken token)
        {
            foreach(var type in types.OrderBy(tt => tt.Priority))
            {
                var automaton = type.Automaton;
                HashSet<int> previousStates = null;
                var states = new HashSet<int>() { automaton.StartState };
                var length = 0;
                for(var charIndex = index; states.Any() && charIndex < input.Length; charIndex++)
                {
                    previousStates = states;
                    states = new HashSet<int>(automaton.Step(states, input[charIndex]));
                    length++;
                }
                if (!states.Any() && previousStates.Any(s => automaton.AcceptingStates.Contains(s)))
                {
                    token = new Token(type, input.Substring(index, length-1), index);
                    return true;
                }
                else if(states.Any(s => automaton.AcceptingStates.Contains(s)))
                {
                    token = new Token(type, input.Substring(index, length), index);
                    return true;
                }
            }
            token = null;
            return false;
        }
    }
}
