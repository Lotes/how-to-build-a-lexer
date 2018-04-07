using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexer.Automaton
{
    public interface ITransitionTargets: ILookup<ICharSet, int>
    {
        bool Contains(char c);
        bool ContainsEpsilon();
    }

    public static class TransitionTargetsExtensions
    {
        public static IEnumerable<int> ReadChar(this ITransitionTargets _this, char key)
        {
            return _this.Where(g => g.Key != null && g.Key.Contains(key)).SelectMany(g => g).ToArray();
        }
        
        public static IEnumerable<int> ReadSet(this ITransitionTargets @this, ICharSet set)
        {
            foreach (var cs in @this)
                if (cs.Key.Contains(set))
                    return cs;
            return Enumerable.Empty<int>();
        }
    }
}
