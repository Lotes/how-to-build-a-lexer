using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Lexer.Automaton
{
    public class CharSet: IEnumerable<CharRange>
    {
        public const char Epsilon = '\0';

        private readonly List<CharRange> list = new List<CharRange>();

        public CharSet(params char[] characters)
        {
            foreach (var c in characters)
                Add(c);
        }

        public int Length { get { return list.Sum(r => r.Count()); } }

        public bool Contains(char c)
        {
            var index = 0;
            while (index < list.Count && c > list[index].To)
                index++;
            if (index >= list.Count)
                return false;
            var range = list[index];
            return c >= range.From && c <= range.To;
        }
        
        public bool Contains(char from, char to)
        {
            var index = 0;
            while (index < list.Count && from > list[index].To)
                index++;
            if (index >= list.Count)
                return false;
            var range = list[index];
            return from >= range.From && to <= range.To;
        }
        
        public void Add(char c)
        {
            var index = 0;
            while (index < list.Count && c > list[index].To)
                index++;
            if (index >= list.Count)
            {
                list.Add(new CharRange(c));
                TryMergeRange(index);
            }
            else
            {
                var range = list[index];
                if(c >= range.From && c <= range.To)
                    return; //already included
                if (c > range.To)
                {
                    list.Insert(index + 1, new CharRange(c));
                    TryMergeRange(index + 1);
                }    
                else // c < range.From
                {
                    list.Insert(index, new CharRange(c));
                    TryMergeRange(index);
                }
            }
        }

        public void Add(char from, char to)
        {
            var index = 0;
            while (index < list.Count && from > list[index].To)
                index++;
            if (index >= list.Count)
            {
                list.Add(new CharRange(from, to));
                TryMergeRange(index);
            }
            else
            {
                var fromIndex = index;
                while (index < list.Count && from > list[index].To)
                    index++;
                if(index < list.Count)
                {
                    if (from > list[index].From)
                    {
                        from = list[index].From;
                        list.RemoveRange(fromIndex, index - fromIndex + 1);
                    }
                }
                list.Insert(fromIndex, new CharRange(from, to));
                TryMergeRange(fromIndex);
            }
        }

        public void Remove(char c)
        {
            var index = 0;
            while (index < list.Count && c > list[index].To)
                index++;
            if (index >= list.Count)
                return;
            var range = list[index];
            if (c < range.From || c > range.To)
                return;
            list.RemoveAt(index);
            if (range.From == range.To)
                return;
            else if (range.From == c)
                    list.Insert(index, new CharRange((char)(c+1), range.To));
            else if (range.To == c)
                list.Insert(index, new CharRange(range.From, (char)(c - 1)));
            else
            {
                list.Insert(index, new CharRange(range.From, (char)(c - 1)));
                list.Insert(index+1, new CharRange((char)(c + 1), range.To));
            }
        }

        public void Remove(char from, char to)
        {
            var index = 0;
            while (index < list.Count && from > list[index].To)
                index++;
            if (index >= list.Count)
                return;
            var range = list[index];
            if (from > range.To)
                return; //nothing to do
            list.RemoveAt(index);
            //  a-c e-g
            //       ^
            //remove f-i
            if (range.From <= from - 1)
            {
                list.Insert(index, new CharRange(range.From, (char)(from - 1)));
                index++;
                while (index < list.Count && to < list[index].From)
                    list.RemoveAt(index);
                if (index < list.Count)
                {
                    range = list[index];
                    list.RemoveAt(index);
                    list.Insert(index, new CharRange((char)(to + 1), range.To));
                }
            }
            //        b-d   
            //       ^
            //remove a-c
            else //range.From > from-1
            {
                if(to+1 <= range.To)
                    list.Insert(index, new CharRange((char)(to+1), range.To));
            }
        }

        private void TryMergeRange(int index)
        {
            var current = list[index];
            //check after
            if (index + 1 < list.Count)
            {
                var after = list[index + 1];
                if (after.From == current.To + 1)
                {
                    list.RemoveRange(index, 2);
                    list.Insert(index, current = new CharRange(current.From, after.To));
                }
            }
            //check before
            if (index - 1 >= 0)
            {
                var before = list[index - 1];
                if (before.To + 1 == current.From)
                {
                    list.RemoveRange(index-1, 2);
                    list.Insert(index-1, new CharRange(before.From, current.To));
                }
            }
        }

        public IEnumerator<CharRange> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return string.Join(",", list.Select(r => r.ToString()));
        }
    }
}