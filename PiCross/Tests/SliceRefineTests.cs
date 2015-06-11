using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PiCross.Game;

namespace PiCross.Tests
{
    [TestClass]
    public class SliceRefineTests
    {
        [TestMethod]
        public void UU_1()
        {
            var slice = CreateSlice( "??" );
            var constraints = CreateConstraints( 1 );
            var expected = CreateSlice( "??" );
            var actual = slice.Refine( constraints );

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        public void UU_2()
        {
            var slice = CreateSlice( "??" );
            var constraints = CreateConstraints( 2 );
            var expected = CreateSlice( "xx" );
            var actual = slice.Refine( constraints );

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        public void UX_1()
        {
            var slice = CreateSlice( "?x" );
            var constraints = CreateConstraints( 1 );
            var expected = CreateSlice( ".x" );
            var actual = slice.Refine( constraints );

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        public void EU_1()
        {
            var slice = CreateSlice( ".?" );
            var constraints = CreateConstraints( 1 );
            var expected = CreateSlice( ".x" );
            var actual = slice.Refine( constraints );

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        public void UUU_2()
        {
            var slice = CreateSlice( "???" );
            var constraints = CreateConstraints( 2 );
            var expected = CreateSlice( "?x?" );
            var actual = slice.Refine( constraints );

            Assert.AreEqual( expected, actual );
        }

        private Constraints CreateConstraints( params int[] values )
        {
            return new Constraints( values );
        }

        private Slice CreateSlice( string str )
        {
            return Slice.FromString( str );
        }
    }
}
