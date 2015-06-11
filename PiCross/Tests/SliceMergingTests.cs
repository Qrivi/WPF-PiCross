using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PiCross.Game;

namespace PiCross.Tests
{
    [TestClass]
    public class SliceMergingTests
    {
        [TestMethod]
        public void Merge_E_E()
        {
            Test( ".", ".", "." );
        }

        [TestMethod]
        public void Merge_X_X()
        {
            Test( "x", "x", "x" );
        }

        [TestMethod]
        public void Merge_X_E()
        {
            Test( "x", ".", "?" );
        }

        [TestMethod]
        public void Merge_E_X()
        {
            Test( ".", "x", "?" );
        }

        [TestMethod]
        public void Merge_U_X()
        {
            Test( "?", "x", "?" );
        }

        [TestMethod]
        public void Merge_U_E()
        {
            Test( "?", ".", "?" );
        }

        [TestMethod]
        public void Merge_X_U()
        {
            Test( "x", "?", "?" );
        }

        [TestMethod]
        public void Merge_E_U()
        {
            Test( ".", "?", "?" );
        }

        [TestMethod]
        public void Merge_XXXEEEUUU_XEUXEUXEU()
        {
            Test( "xxx...???", "x.?x.?x.?", "x???.????" );
        }

        private void Test(string sliceString1, string sliceString2, string expectedString)
        {
            var slice1 = Slice.FromString( sliceString1 );
            var slice2 = Slice.FromString( sliceString2 );
            var actual = slice1.Merge( slice2 );
            var expected  = Slice.FromString( expectedString );

            Assert.AreEqual( expected, actual );
        }
    }
}
