using System.Collections.Generic;

namespace Lexer.Automaton
{
    public interface ICharSet : IEnumerable<CharRange>
    {
        int Length { get; }
        bool Contains(char c);
        bool Contains(char from, char to);
    }
}