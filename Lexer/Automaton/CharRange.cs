using System;
using System.Collections;
using System.Collections.Generic;

namespace Lexer.Automaton
{
    public class CharRange: IEnumerable<char>
    {
        public CharRange(char single)
            : this(single, single) {}
        
        public CharRange(char from, char to)
        {
            if(from > to)
                throw  new ArgumentException(nameof(from));
            From = from;
            To = to;
        }
 
        public char From { get; }
        public char To { get; }
        
        public bool Contains(char c)
        {
            return c >= From && c <= To;
        }
        
        public IEnumerator<char> GetEnumerator()
        {
            for(char index=From; index<=To; index++)
                yield return index;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override bool Equals(object obj)
        {
            if(!(obj is CharRange other))
                return false;
            return this.From == other.From && this.To == other.To;
        }

        public override int GetHashCode()
        {
            return (byte)From | ((byte)To << 8);
        }
    }
}