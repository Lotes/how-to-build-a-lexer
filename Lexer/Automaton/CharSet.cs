using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Lexer.Automaton
{
    public class CharSet: ICharSet
    {
        public static readonly ICharSet Epsilon = null;
        public static readonly ICharSet Full = new CharSet(new CharRange('\0', '\uFFFF', SetMode.Included));
        public static readonly ICharSet Empty = new CharSet();

        protected readonly List<CharRange> list = new List<CharRange>();

        public CharSet() { list.Add(new CharRange('\0', '\uFFFF', SetMode.Excluded)); }
        public CharSet(ICharSet from)
            : this()
        {
            foreach (var range in from)
                Add(range.From, range.To);
        }

        public CharSet(params char[] characters)
            : this()
        {
            foreach (var c in characters)
                Add(c);
        }

        public CharSet(params CharRange[] ranges)
            : this()
        {
            foreach (var range in ranges)
                Add(range.From, range.To);
        }

        public int Length
        {
            get
            {
                return list.Where(r => r.Mode == SetMode.Included)
                    .Sum(r => (int)r.To - (int)r.From + 1);
            }
        }

        private bool IsXXcluded(SetMode mode, char from, char to)
        {
            var index = 0;
            while (index < list.Count && from > list[index].To)
                index++;
            if (index >= list.Count)
                return false;
            var range = list[index];
            return from >= range.From && to <= range.To && range.Mode == mode;
        }

        public bool Includes(char c)
        {
            return Includes(c, c);
        }
        
        public bool Includes(char from, char to)
        {
            return IsXXcluded(SetMode.Included, from, to);
        }

        public bool Excludes(char c)
        {
            return Excludes(c, c);
        }

        public bool Excludes(char from, char to)
        {
            return IsXXcluded(SetMode.Excluded, from, to);
        }

        private void Change(SetMode mode, char from, char to)
        {
            if (from > to)
                throw new ArgumentException("Range limit order is invalid!", nameof(from));

            var leftMostIndex = 0;
            while (leftMostIndex < list.Count && from > list[leftMostIndex].To)
                leftMostIndex++;

            var rightMostIndex = list.Count - 1;
            while (rightMostIndex >= 0 && to < list[rightMostIndex].From)
                rightMostIndex--;

            var leftMostRange = list[leftMostIndex];
            var rightMostRange = list[rightMostIndex];
            var leftMost = leftMostRange.From;
            var rightMost = rightMostRange.To;
            list.RemoveRange(leftMostIndex, rightMostIndex - leftMostIndex + 1);

            if(leftMost < from)
            {
                if(rightMost > to)
                {
                    list.InsertRange(leftMostIndex, new[] 
                    {
                        new CharRange(leftMost, (char)(from-1), leftMostRange.Mode),
                        new CharRange(from, to, mode),
                        new CharRange((char)(to+1), rightMost, rightMostRange.Mode)
                    });
                }
                else
                {
                    list.InsertRange(leftMostIndex, new[]
                    {
                        new CharRange(leftMost, (char)(from - 1), leftMostRange.Mode),
                        new CharRange(from, to, mode)
                    });
                }
            }
            else
            {
                if (rightMost > to)
                {
                    list.InsertRange(leftMostIndex, new[]
                    {
                        new CharRange(from, to, mode),
                        new CharRange((char)(to + 1), rightMost, rightMostRange.Mode)
                    });
                }
                else
                {
                    list.InsertRange(leftMostIndex, new[]
                    {
                        new CharRange(from, to, mode)
                    });
                }
            }

            TryMergeRange(Math.Max(0, leftMostIndex-1), 4);
        }

        public void Add(char c)
        {
            Add(c, c);
        }

        public void Add(char from, char to)
        {
            Change(SetMode.Included, from, to);
        }

        public void Remove(char c)
        {
            Remove(c, c);
        }

        public void Remove(char from, char to)
        {
            Change(SetMode.Excluded, from, to);
        }

        private void TryMergeRange(int fromRange, int toRange)
        {
            var index = fromRange;
            while(index < toRange && index + 1 < list.Count)
            {
                var current = list[index];
                var next = list[index+1];
                if (current.Mode == next.Mode && current.To + 1 == next.From)
                {
                    list.RemoveRange(index, 2);
                    list.Insert(index, new CharRange(current.From, next.To, current.Mode));
                }
                else
                    index++;
            }
        }

        public IEnumerator<CharRange> GetEnumerator()
        {
            return list.Where(r => r.Mode == SetMode.Included).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return string.Join(",", list.Select(r => r.ToString()));
        }

        public override int GetHashCode()
        {
            return list.Aggregate(0, (sum, original) => sum * 13 + original.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            var other = obj as ICharSet;
            if (other == null || this.Count() !=  other.Count())
                return false;
            return this.Zip(other, (lhs, rhs) => lhs.Equals(rhs))
                .Aggregate(true, (lhs, rhs) => lhs && rhs);
        }

        public int CompareTo(ICharSet other)
        {
            var x = this;
            var y = other;
            if (x == null && y == null)
                return 0;
            if (x == null)
                return -1;
            if (y == null)
                return +1;
            if (x.Length == 0 && y.Length == 0)
                return 0;
            if (x.Length == 0)
                return -1;
            if (y.Length == 0)
                return +1;
            var lhs = x.First().First();
            var rhs = y.First().First();
            return lhs.CompareTo(rhs);
        }
    }
}