using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PiCross.Game;

namespace PiCross.Tests
{
    [TestClass]
    public class SliceDeriveConstraintsTests
    {
        [TestMethod]
        public void E()
        {
            new Test() { Slice = ".", Expected = CreateConstraints() };
        }

        [TestMethod]
        public void EE()
        {
            new Test() { Slice = "..", Expected = CreateConstraints() };
        }

        [TestMethod]
        public void X()
        {
            new Test() { Slice = "x", Expected = CreateConstraints( 1 ) };
        }

        [TestMethod]
        public void UX()
        {
            new Test() { Slice = ".x", Expected = CreateConstraints( 1 ) };
        }

        [TestMethod]
        public void XU()
        {
            new Test() { Slice = "x.", Expected = CreateConstraints( 1 ) };
        }

        [TestMethod]
        public void XX()
        {
            new Test() { Slice = "xx", Expected = CreateConstraints( 2 ) };
        }

        [TestMethod]
        public void UXX()
        {
            new Test() { Slice = ".xx", Expected = CreateConstraints( 2 ) };
        }

        [TestMethod]
        public void XXU()
        {
            new Test() { Slice = "xx.", Expected = CreateConstraints( 2 ) };
        }

        [TestMethod]
        public void XXEXX()
        {
            new Test() { Slice = "xx.xx", Expected = CreateConstraints( 2, 2 ) };
        }


        [TestMethod]
        public void XXEXXE()
        {
            new Test() { Slice = "xx.xx.", Expected = CreateConstraints( 2, 2 ) };
        }


        [TestMethod]
        public void EXXEXX()
        {
            new Test() { Slice = ".xx.xx", Expected = CreateConstraints( 2, 2 ) };
        }


        [TestMethod]
        public void EXXEXXE()
        {
            new Test() { Slice = ".xx.xx.", Expected = CreateConstraints( 2, 2 ) };
        }


        [TestMethod]
        public void XXEEEXX()
        {
            new Test() { Slice = "xx...xx", Expected = CreateConstraints( 2, 2 ) };
        }

        private static Slice CreateSlice( string str )
        {
            return Slice.FromString( str );
        }

        private static Constraints CreateConstraints( params int[] values )
        {
            return new Constraints( values );
        }

        private class Test
        {
            private Slice slice;

            private Constraints expected;

            public string Slice
            {
                set
                {
                    this.slice = CreateSlice( value );
                }
            }

            public Constraints Expected
            {
                set
                {
                    this.expected = value;
                }
            }

            public void Check()
            {
                var actual = slice.DeriveConstraints();

                Assert.AreEqual( expected, actual );
            }
        }
    }
}
