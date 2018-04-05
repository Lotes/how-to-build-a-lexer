using Lexer.Automaton;

namespace Lexer.RegularExpression
{
    public interface IParser
    {
        IAutomaton Parse(string input);
    }
}