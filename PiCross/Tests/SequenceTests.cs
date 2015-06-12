using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PiCross.DataStructures;

namespace PiCross.Tests
{
    [TestClass]
    public class SequenceTests
    {
        [TestMethod]
        [TestCategory( "Sequence" )]
        public void CreateEmpty()
        {
            var seq = Sequence.CreateEmpty<int>();

            Assert.AreEqual( 0, seq.Length );
        }

        [TestMethod]
        [TestCategory("Sequence")]
        public void Concatenation()
        {
            var xs = Sequence.FromItems( 1, 2, 3 );
            var ys = Sequence.FromItems( 4, 5 );
            var actual = xs.Concatenate( ys );
            var expected = Sequence.FromItems( 1, 2, 3, 4, 5 );

            Assert.AreEqual( expected, actual );
        }
    }
}
