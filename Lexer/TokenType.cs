using Lexer.Automaton;

namespace Lexer
{
    public class TokenType : ITokenType
    {
        public TokenType(string name, IAutomaton automaton, int priority)
        {
            Name = name;
            Automaton = automaton;
            Priority = priority;
        }

        public IAutomaton Automaton { get; }

        public string Name { get; }

        public int Priority { get; }
    }
}
