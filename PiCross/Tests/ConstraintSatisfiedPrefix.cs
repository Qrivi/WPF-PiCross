using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PiCross.Game;

namespace PiCross.Tests
{
    [TestClass]
    public class ConstraintSatisfiedPrefixLengthTests
    {
        [TestMethod]
        public void Test1()
        {
            var constraints = CreateConstraints( 1, 1 );
            var slice = CreateSlice( "x.?" );
            var actual = constraints.SatisfiedPrefixLength( slice );
            var expected = 1;

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        public void Test2()
        {
            var constraints = CreateConstraints( 1, 1 );
            var slice = CreateSlice( "???" );
            var actual = constraints.SatisfiedPrefixLength( slice );
            var expected = 0;

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        public void Test3()
        {
            var constraints = CreateConstraints( 1, 1 );
            var slice = CreateSlice( "x??" );
            var actual = constraints.SatisfiedPrefixLength( slice );
            var expected = 1;

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        public void Test4()
        {
            var constraints = CreateConstraints( 1, 1 );
            var slice = CreateSlice( ".x.?" );
            var actual = constraints.SatisfiedPrefixLength( slice );
            var expected = 1;

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        public void Test5()
        {
            var constraints = CreateConstraints( 1, 1 );
            var slice = CreateSlice( ".xx.?" );
            var actual = constraints.SatisfiedPrefixLength( slice );
            var expected = 0;

            Assert.AreEqual( expected, actual );
        }

        private static Slice CreateSlice(string str)
        {
            return Slice.FromString( str );
        }

        private static Constraints CreateConstraints(params int[] constraints)
        {
            return new Constraints( constraints );
        }
    }
}
