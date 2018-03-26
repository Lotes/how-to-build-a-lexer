using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Lexer.Automaton
{
    public class CharSet: IEnumerable<CharRange>
    {
        private readonly List<CharRange> list = new List<CharRange>();

        public bool Contains(char c)
        {
            var index = 0;
            while (index < list.Count && list[index].From < c)
                index++;
            if (index >= list.Count)
                return false;
            var range = list[index];
            return c >= range.From && c <= range.To;
        }
        
        public bool Contains(char from, char to)
        {
            var index = 0;
            while (index < list.Count && list[index].From < from)
                index++;
            if (index >= list.Count)
                return false;
            var range = list[index];
            return from >= range.From && to <= range.To;
        }
        
        public void Add(char c)
        {
            var index = 0;
            while (index < list.Count && list[index].From < c)
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
                //c must be greater than range.To
                list.Insert(index+1, new CharRange(c));
                TryMergeRange(index+1);
            }
        }

        public void Add(char from, char to)
        {
            var index = 0;
            while (index < list.Count && list[index].From < from)
                index++;
            if (index >= list.Count)
            {
                list.Add(new CharRange(from, to));
                TryMergeRange(index);
            }
            else
            {
                var fromIndex = index;
                while (index < list.Count && list[index].To < to)
                    index++;
                list.RemoveRange(fromIndex, index-fromIndex+1);
                list.Insert(fromIndex, new CharRange(from, to));
                TryMergeRange(fromIndex);
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
    }
}