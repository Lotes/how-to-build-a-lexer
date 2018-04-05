using Lexer.Automaton;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.UnitTests
{
    [TestClass]
    public class ConstructionKitTests
    {
        [TestMethod]
        public void StepCharacter()
        {
            var automaton = ConstructionKit.Consume(new[] { 'a', 'b', 'c' });
            automaton.Accepts("a");
            automaton.Accepts("b");
            automaton.Accepts("c");
            automaton.Rejects("d");
        }

        [TestMethod]
        public void StepConcatenation()
        {
            var A = ConstructionKit.Consume(new[] { 'a' });
            var B = ConstructionKit.Consume(new[] { 'b' });
            var automaton = ConstructionKit.Concat(new[] { A, B });
            automaton.Accepts("ab");
            automaton.Rejects("ba");
        }

        [TestMethod]
        public void StepAlternation()
        {
            var A = ConstructionKit.Consume(new[] { 'a' });
            var B = ConstructionKit.Consume(new[] { 'b' });
            var AA = ConstructionKit.Concat(new[] { A, A });
            var automaton = ConstructionKit.Alternate(new[] { AA, B });
            automaton.Accepts("b");
            automaton.Accepts("aa");
            automaton.Rejects("bb");
            automaton.Rejects("a");
        }

        [TestMethod]
        public void StepRepetition()
        {
            var A = ConstructionKit.Consume(new[] { 'a' });
            var automaton = ConstructionKit.Repeat(A);
            automaton.Accepts("");
            automaton.Accepts("a");
            automaton.Accepts("aa");
            automaton.Accepts("aaa");
            automaton.Rejects("b");
        }

        [TestMethod]
        public void StepDeterminize()
        {
            var A = ConstructionKit.Consume(new[] { 'a' });
            var B = ConstructionKit.Consume(new[] { 'b' });
            var AA = ConstructionKit.Concat(new[] { A, A });
            var AAorB = ConstructionKit.Alternate(new[] { AA, B });
            var automaton = ConstructionKit.Repeat(AAorB)
                .Determinize();
            automaton.Accepts("");
            automaton.Accepts("aa");
            automaton.Accepts("aa");
            automaton.Accepts("aaaa");
            automaton.Accepts("baab");
            automaton.Accepts("baabaabb");
            automaton.Rejects("a");
            automaton.Rejects("aaa");
        }

        [TestMethod]
        public void StepMinimize()
        {
            var A = ConstructionKit.Consume(new[] { 'a' });
            var B = ConstructionKit.Consume(new[] { 'b' });
            var AA = ConstructionKit.Concat(new[] { A, A });
            var AAorB = ConstructionKit.Alternate(new[] { AA, B });
            var automaton = ConstructionKit.Repeat(AAorB)
                .Determinize()
                .Minimize();
            automaton.Accepts("");
            automaton.Accepts("aa");
            automaton.Accepts("aa");
            automaton.Accepts("aaaa");
            automaton.Accepts("baab");
            automaton.Accepts("baabaabb");
            automaton.Rejects("a");
            automaton.Rejects("aaa");
        }
    }
}
