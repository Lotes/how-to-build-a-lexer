using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexer.Automaton.Impl
{
    public class TransitionTargets : ITransitionTargets
    {
        public class Node : IGrouping<ICharSet, int>
        {
            private LinkedList<int> values = new LinkedList<int>();

            public Node(ICharSet key, params int[] states)
            {
                Key = key;
                foreach (var state in states)
                    values.AddLast(state);
            }

            public void Add(int value) { values.AddLast(value); }

            public ICharSet Key { get; private set; }

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
        private SortedList<ICharSet, Node> nodes = new SortedList<ICharSet, Node>();

        public void Add(ICharSet key, int value)
        {
            if (key == null)
            {
                if (nullNode == null)
                    nullNode = new Node(null);
                nullNode.Add(value);
            }
            else
            {
                if (key.Length == 0)
                    return;
                var added = false;
                for (var index = 0; index < nodes.Count; index++)
                {
                    var charset = nodes.Keys[index];
                    var node = nodes.Values[index];
                    if (charset == null)
                        continue;
                    var cmnKey = key.Intersect(charset);
                    if (cmnKey.Any())
                    {
                        if(key.Equals(charset))
                        {
                            added = true;
                            node.Add(value);
                            break;
                        }
                        else
                        {
                            added = true;
                            nodes.RemoveAt(index);

                            var lhsKey = key.Except(charset);
                            var rhsKey = charset.Except(key);

                            nodes.Add(cmnKey, new Node(cmnKey, node.Prepend(value).ToArray()));
                            if (lhsKey.Any())
                            {
                                nodes.Add(lhsKey, new Node(lhsKey, value));
                                index++;
                            }
                            if (rhsKey.Any())
                            {
                                nodes.Add(rhsKey, new Node(rhsKey, node.ToArray()));
                                index++;
                            }
                        }
                    }
                }
                if (!added)
                    nodes.Add(key, new Node(key, value));
            }
        }

        public IEnumerable<int> this[ICharSet key]
        {
            get { return key == null ? nullNode : nodes.ContainsKey(key) ? nodes[key] : Enumerable.Empty<int>(); }
        }

        public int Count => nodes.Count;

        public bool Contains(ICharSet key)
        {
            return key == null ? nullNode != null : nodes.ContainsKey(key);
        }

        public IEnumerator<IGrouping<ICharSet, int>> GetEnumerator()
        {
            var result = (IEnumerable<IGrouping<ICharSet, int>>)nodes.Values;
            if (nullNode != null)
                result = result.Prepend(nullNode);
            return result.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Contains(char c)
        {
            foreach (var key in nodes.Keys)
                if (key.Includes(c))
                    return true;
            return false;
        }

        public bool ContainsEpsilon()
        {
            return nullNode != null;
        }
    }
}
