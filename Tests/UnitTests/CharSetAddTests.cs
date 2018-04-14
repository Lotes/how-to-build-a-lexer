using Lexer.Automaton;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests.UnitTests
{
    [TestClass]
    public class CharSetAddTests
    {
        [TestMethod]
        public void EmptySet_AddSingleCharacter_Success()
        {
            var c = 'a';
            var charSet = new CharSet();
            charSet.Add(c);
            Assert.AreEqual(1, charSet.Length);
            Assert.IsTrue(charSet.Includes(c));
        }

        [TestMethod]
        public void EmptySet_AddCharacterRange_Success()
        {
            var cFrom = 'a';
            var cTo = 'c';
            var charSet = new CharSet();
            charSet.Add(cFrom, cTo);
            Assert.AreEqual(3, charSet.Length);
            Assert.IsTrue(charSet.Includes(cFrom, cTo));
            Assert.IsTrue(charSet.Includes(cFrom));
            Assert.IsTrue(charSet.Includes(++cFrom));
            Assert.IsTrue(charSet.Includes(++cFrom));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void EmptySet_AddBadCharacterRange_Fail()
        {
            var charSet = new CharSet();
            charSet.Add('c', 'a');
        }

        [TestMethod]
        public void OneCharSet_AddSingleCharacterSame_Success()
        {
            var charSet = new CharSet();
            charSet.Add('a');

            charSet.Add('a');

            Assert.AreEqual(1, charSet.Length);
            Assert.IsTrue(charSet.Includes('a'));
        }

        [TestMethod]
        public void OneCharSet_AddSingleCharacterBeside_Success()
        {
            var charSet = new CharSet();
            charSet.Add('a');

            charSet.Add('b');

            Assert.AreEqual(2, charSet.Length);
            Assert.IsTrue(charSet.Includes('a', 'b'));
        }

        [TestMethod]
        public void OneCharSet_AddSingleCharacterWithGap_Success()
        {
            var charSet = new CharSet();
            charSet.Add('a');

            charSet.Add('c');

            Assert.AreEqual(2, charSet.Length);
            Assert.IsFalse(charSet.Includes('a', 'c'));
            Assert.IsTrue(charSet.Includes('a'));
            Assert.IsTrue(charSet.Includes('c'));
        }

        [TestMethod]
        public void OneCharSet_AddCharacterRangeBeside_Success()
        {
            var charSet = new CharSet();
            charSet.Add('a', 'c');

            charSet.Add('d', 'f');

            Assert.AreEqual(6, charSet.Length);
            Assert.IsTrue(charSet.Includes('a', 'f'));
        }

        [TestMethod]
        public void OneCharSet_AddCharacterRangeWithGap_Success()
        {
            var charSet = new CharSet();
            charSet.Add('a', 'c');

            charSet.Add('e', 'g');

            Assert.AreEqual(6, charSet.Length);
            Assert.IsFalse(charSet.Includes('a', 'g'));
            Assert.IsTrue(charSet.Includes('e', 'g'));
        }

        [TestMethod]
        public void ThreeCharsSet_AddSingleCharacterAlreadyExists_Success()
        {
            var charSet = new CharSet();
            charSet.Add('a', 'c');

            charSet.Add('b');

            Assert.AreEqual(3, charSet.Length);
            Assert.IsTrue(charSet.Includes('a', 'c'));
        }

        [TestMethod]
        public void ThreeCharsSet_AddSingleCharacterBeside_Success()
        {
            var charSet = new CharSet();
            charSet.Add('a', 'c');

            charSet.Add('d');

            Assert.AreEqual(4, charSet.Length);
            Assert.IsTrue(charSet.Includes('a', 'd'));
        }

        [TestMethod]
        public void ThreeCharsSet_AddSingleCharacterWithGap_Success()
        {
            var charSet = new CharSet();
            charSet.Add('a', 'c');

            charSet.Add('e');

            Assert.AreEqual(4, charSet.Length);
            Assert.IsFalse(charSet.Includes('a', 'e'));
            Assert.IsTrue(charSet.Includes('e'));
        }

        [TestMethod]
        public void ThreeCharsSet_AddCharacterRangeWithin_Success()
        {
            var charSet = new CharSet();
            charSet.Add('a', 'c');

            charSet.Add('b', 'c');

            Assert.AreEqual(3, charSet.Length);
            Assert.IsTrue(charSet.Includes('a', 'c'));
        }

        [TestMethod]
        public void ThreeCharsSet_AddCharacterRangeOverlapping_Success()
        {
            var charSet = new CharSet();
            charSet.Add('a', 'c');

            charSet.Add('b', 'd');

            Assert.AreEqual(4, charSet.Length);
            Assert.IsTrue(charSet.Includes('a', 'd'));
        }

        [TestMethod]
        public void ThreeCharsSet_AddCharacterRangeBeside_Success()
        {
            var charSet = new CharSet();
            charSet.Add('a', 'c');

            charSet.Add('d', 'f');

            Assert.AreEqual(6, charSet.Length);
            Assert.IsTrue(charSet.Includes('a', 'f'));
        }

        [TestMethod]
        public void ThreeCharsSet_AddCharacterRangeWithGap_Success()
        {
            var charSet = new CharSet();
            charSet.Add('a', 'c');

            charSet.Add('e', 'g');

            Assert.AreEqual(6, charSet.Length);
            Assert.IsFalse(charSet.Includes('a', 'g'));
            Assert.IsTrue(charSet.Includes('e', 'g'));
        }

        [TestMethod]
        public void TwoThreeGroupsCharsSet_AddCharacterFillGap_Success()
        {
            var charSet = new CharSet();
            charSet.Add('a', 'c');
            charSet.Add('e', 'g');

            charSet.Add('d');

            Assert.AreEqual(7, charSet.Length);
            Assert.IsTrue(charSet.Includes('a', 'g'));
        }

        [TestMethod]
        public void TwoThreeGroupsCharsSet_AddCharacterRangeFillGap_Success()
        {
            var charSet = new CharSet();
            charSet.Add('a', 'c');
            charSet.Add('g', 'i');

            charSet.Add('d', 'f');

            Assert.AreEqual(9, charSet.Length);
            Assert.IsTrue(charSet.Includes('a', 'i'));
        }
    }
}