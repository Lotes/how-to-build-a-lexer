using Lexer.Automaton;

namespace Lexer
{
    public interface ITokenType
    {
        IAutomaton Automaton { get; }
        string Name { get; }
        int Priority { get; }
    }
}
