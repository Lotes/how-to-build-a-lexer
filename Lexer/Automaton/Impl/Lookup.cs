using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexer.Automaton.Impl
{
    public class Lookup : ILookup<char?, int>
    {
        public class Node : IGrouping<char?, int>
        {
            private LinkedList<int> values = new LinkedList<int>();

            public Node(char? key)
            {
                Key = key;
            }

            public void Add(int value) { values.AddLast(value); }

            public char? Key { get; private set; }

            public IEnumerator<int> GetEnumerator()
            {
                return values.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private Node nullNode = null;
        private Dictionary<char, Node> nodes = new Dictionary<char, Node>();

        public void Add(char? key, int value)
        {
            if(key == null)
            {
                if (nullNode == null)
                    nullNode = new Node(null);
                nullNode.Add(value);
            }
            else
            {
                if (!nodes.ContainsKey(key.Value))
                    nodes.Add(key.Value, new Node(key.Value));
                nodes[key.Value].Add(value);
            }
        }

        public IEnumerable<int> this[char? key]
        {
            get { return key == null ? nullNode : nodes.ContainsKey(key.Value) ? nodes[key.Value] : null; }
        }

        public int Count => nodes.Count;

        public bool Contains(char? key)
        {
            return key == null ? nullNode != null : nodes.ContainsKey(key.Value);
        }

        public IEnumerator<IGrouping<char?, int>> GetEnumerator()
        {
            var result = (IEnumerable<IGrouping<char?, int>>)nodes.Values;
            if (nullNode != null)
                result = result.Prepend(nullNode);
            return result.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
