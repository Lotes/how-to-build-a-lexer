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
    public class CharSetRemoveTests
    {
        [TestMethod]
        public void EmptyCharSet_RemoveSingleCharacter_Success()
        {
            var charset = new CharSet();
            charset.Remove('a');
            Assert.AreEqual(0, charset.Length);
        }

        [TestMethod]
        public void EmptyCharSet_RemoveCharacterRange_Success()
        {
            var charset = new CharSet();
            charset.Remove('a', 'c');
            Assert.AreEqual(0, charset.Length);
        }

        [TestMethod]
        public void OneCharSet_RemoveSingleCharacter_Success()
        {
            var charset = new CharSet();
            charset.Add('a');
            charset.Remove('a');
            Assert.AreEqual(0, charset.Length);
        }

        [TestMethod]
        public void OneCharSet_RemoveCharacterRange_Success()
        {
            var charset = new CharSet();
            charset.Add('b');
            charset.Remove('a', 'c');
            Assert.AreEqual(0, charset.Length);
        }

        [TestMethod]
        public void ThreeCharsSet_RemoveSingleCharacterMiddle_Success()
        {
            var charset = new CharSet();
            charset.Add('a', 'c');
            charset.Remove('b');
            Assert.AreEqual(2, charset.Length);
            Assert.IsTrue(charset.Contains('a'));
            Assert.IsTrue(charset.Contains('c'));
        }

        [TestMethod]
        public void ThreeCharsSet_RemoveSingleCharacterLeft_Success()
        {
            var charset = new CharSet();
            charset.Add('a', 'c');
            charset.Remove('a');
            Assert.AreEqual(2, charset.Length);
            Assert.IsTrue(charset.Contains('b'));
            Assert.IsTrue(charset.Contains('c'));
        }

        [TestMethod]
        public void ThreeCharsSet_RemoveSingleCharacterRight_Success()
        {
            var charset = new CharSet();
            charset.Add('a', 'c');
            charset.Remove('c');
            Assert.AreEqual(2, charset.Length);
            Assert.IsTrue(charset.Contains('a'));
            Assert.IsTrue(charset.Contains('b'));
        }

        [TestMethod]
        public void ThreeCharsSet_RemoveCharacterRangeExact_Success()
        {
            var charset = new CharSet();
            charset.Add('a', 'c');
            charset.Remove('a', 'c');
            Assert.AreEqual(0, charset.Length);
        }

        [TestMethod]
        public void ThreeCharsSet_RemoveCharacterRangeOverlapRight_Success()
        {
            var charset = new CharSet();
            charset.Add('a', 'c');
            charset.Remove('b', 'd');
            Assert.AreEqual(1, charset.Length);
            Assert.IsTrue(charset.Contains('a'));
        }

        [TestMethod]
        public void ThreeCharsSet_RemoveCharacterRangeOverlapLeft_Success()
        {
            var charset = new CharSet();
            charset.Add('b', 'd');
            charset.Remove('a', 'c');
            Assert.AreEqual(1, charset.Length);
            Assert.IsTrue(charset.Contains('d'));
        }
    }
}
