using Lexer.Automaton;

namespace Lexer
{
    public interface IRegexParser
    {
        IAutomaton Parse(string input);
    }
}