using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexer.Automaton
{
    public static class CharSetExtensions
    {
        public static bool Contains(this ICharSet @this, ICharSet set)
        {
            if (@this == null)
                return false;
            foreach (var range in set)
                if (!@this.Contains(range))
                    return false;
            return true;
        }
        public static void Add(this CharSet @this, ICharSet other)
        {
            foreach (var range in other)
                @this.Add(range.From, range.To);
        }
        public static void Remove(this CharSet @this, ICharSet other)
        {
            foreach (var range in other)
                @this.Remove(range.From, range.To);
        }

        public static ICharSet Union(this ICharSet @this, params ICharSet[] others)
        {
            var result = new CharSet(@this);
            foreach(var other in others)
                result.Add(other);
            return result;
        }

        public static ICharSet Except(this ICharSet @this, ICharSet other)
        {
            var result = new CharSet(@this);
            result.Remove(other);
            return result;
        }

        public static ICharSet Negate(this ICharSet @this)
        {
            return CharSet.Full.Except(@this);
        }

        public static ICharSet Intersect(this ICharSet @this, ICharSet other)
        {
            //A and B = !(!A or !B)
            var notA = @this.Negate();
            var notB = other.Negate();
            return notA.Union(notB).Negate();
        }
    }
}
