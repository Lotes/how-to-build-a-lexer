using System.Collections.Generic;

namespace Lexer.Automaton
{
    public interface ICharSet : IEnumerable<CharRange>, IComparer<ICharSet>
    {
        int Length { get; }
        bool Contains(char c);
        bool Contains(char from, char to);
    }
}