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
    public class CharSetOpsTests
    {
        [TestMethod]
        public void Union_TwoEmptySets_EmptyResult()
        {
            var set = CharSet.Empty.Union(CharSet.Empty);
            Assert.AreEqual(0, set.Length);
        }

        [TestMethod]
        public void Union_TwoSetsLeftEmpty_RightResult()
        {
            var lhs = CharSet.Empty;
            var rhs = new CharSet('a', 'b', 'c');
            var set = lhs.Union(rhs);
            Assert.AreEqual(rhs, set);
        }

        [TestMethod]
        public void Union_TwoSetsRightEmpty_LeftResult()
        {
            var lhs = new CharSet('a', 'b', 'c');
            var rhs = CharSet.Empty;
            var set = lhs.Union(rhs);
            Assert.AreEqual(lhs, set);
        }

        [TestMethod]
        public void Union_TwoSetsNoOverlap_ResultAll()
        {
            var lhs = new CharSet('a', 'b', 'c');
            var rhs = new CharSet('e', 'f', 'g');
            var set = lhs.Union(rhs);
            Assert.IsTrue(set.Includes('a', 'c'));
            Assert.IsTrue(set.Includes('e', 'g'));
        }

        [TestMethod]
        public void Union_TwoSetsNoOverlapNeighbours_ResultAll()
        {
            var lhs = new CharSet('a', 'b', 'c');
            var rhs = new CharSet('d', 'e', 'f');
            var set = lhs.Union(rhs);
            Assert.IsTrue(set.Includes('a', 'f'));
            Assert.AreEqual(6, set.Length);
        }

        [TestMethod]
        public void Union_TwoSetsOverlap_ResultAll()
        {
            var lhs = new CharSet('a', 'b', 'c');
            var rhs = new CharSet('c', 'd', 'e');
            var set = lhs.Union(rhs);
            Assert.IsTrue(set.Includes('a', 'e'));
            Assert.AreEqual(5, set.Length);
        }

        [TestMethod]
        public void Union_TwoEqualSets_ResultAll()
        {
            var lhs = new CharSet('a', 'b', 'c');
            var set = lhs.Union(lhs);
            Assert.IsTrue(set.Includes('a', 'c'));
            Assert.AreEqual(3, set.Length);
        }

        [TestMethod]
        public void Negate_EmptySet_ResultFullSet()
        {
            var set = CharSet.Empty.Negate();
            Assert.IsTrue(set.Contains(CharSet.Full));
        }

        [TestMethod]
        public void Negate_FullSet_ResultEmptySet()
        {
            var set = CharSet.Full.Negate();
            Assert.AreEqual(CharSet.Empty, set);
        }

        [TestMethod]
        public void Negate_RangeSetLowerEnd_ResultRangeSetUpperEnd()
        {
            var set = new CharSet(new CharRange('\0', 'a')).Negate();
            Assert.AreEqual(new CharSet(new CharRange('b', '\uffff')), set);
        }

        [TestMethod]
        public void Negate_RangeSetUpperEnd_ResultRangeSetLowerEnd()
        {
            var set = new CharSet(new CharRange('b', '\uffff')).Negate();
            Assert.AreEqual(new CharSet(new CharRange('\0', 'a')), set);
        }

        [TestMethod]
        public void Negate_RangeSetMiddle_ResultOuterRangeSets()
        {
            var original = new CharSet(new CharRange('b', 'y'));
            var set = original.Negate();
            Assert.AreEqual(new CharSet(new CharRange('\0', 'a'), new CharRange('z', '\uffff')), set);
        }

        [TestMethod]
        public void Negate_OuterRangeSets_ResultSetMiddle()
        {
            var original = new CharSet(new CharRange('\0', 'a'), new CharRange('z', '\uffff'));
            var set = original.Negate();
            Assert.AreEqual(new CharSet(new CharRange('b', 'y')), set);
        }
        //Intersect
        //Except
    }
}
