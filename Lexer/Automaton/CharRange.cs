using System;
using System.Collections;
using System.Collections.Generic;

namespace Lexer.Automaton
{
    public class CharRange: IEnumerable<char>
    {
        public CharRange(char single)
            : this(single, single) {}
        
        public CharRange(char from, char to, SetMode mode = SetMode.Included)
        {
            if(from > to)
                throw  new ArgumentException(nameof(from));
            From = from;
            To = to;
            Mode = mode;
        }
 
        public SetMode Mode { get; }
        public char From { get; }
        public char To { get; }
        
        public bool Contains(char c)
        {
            return c >= From && c <= To;
        }

        public CharRange Clone()
        {
            return new CharRange(From, To, Mode);
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

        public override string ToString()
        {
            return "[" + (Mode == SetMode.Excluded ? "^" : "") + (From == To ? ((int)From).ToString() : ((int)From).ToString()+"-"+((int)To).ToString()) + "]";
        }
    }
}