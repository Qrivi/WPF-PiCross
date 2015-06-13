using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PiCross.Game;

namespace PiCross.Tests
{
    [TestClass]
    public class Slice_KnownPrefix
    {
        [TestMethod]
        [TestCategory("Slice")]
        public void KnownPrefix_UUU()
        {
            var slice = CreateSlice( "???" );
            var expected = CreateSlice( "" );
            var actual = slice.KnownPrefix;

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Slice" )]
        public void KnownPrefix_XUU()
        {
            var slice = CreateSlice( "x??" );
            var expected = CreateSlice( "" );
            var actual = slice.KnownPrefix;

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Slice" )]
        public void KnownPrefix_XEU()
        {
            var slice = CreateSlice( "x.?" );
            var expected = CreateSlice( "x." );
            var actual = slice.KnownPrefix;

            Assert.AreEqual( expected, actual );
        }

        private static Slice CreateSlice( string str )
        {
            return Slice.FromString( str );
        }
    }
}
