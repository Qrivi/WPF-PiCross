using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PiCross.Game;

namespace PiCross.Tests
{
    [TestClass]
    public class PlayGridConstraints_IsSatisfied_Tests
    {
        [TestMethod]
        [TestCategory( "PlayGridConstraints" )]
        public void IsSatisfied1()
        {
            var pgc = CreatePGConstraints( "???", 1, 1 );

            Assert.IsFalse( pgc.Values[0].IsSatisfied );
            Assert.IsFalse( pgc.Values[1].IsSatisfied );
        }

        [TestMethod]
        [TestCategory( "PlayGridConstraints" )]
        public void IsSatisfied2()
        {
            var pgc = CreatePGConstraints( "x.x", 1, 1 );

            Assert.IsTrue( pgc.Values[0].IsSatisfied );
            Assert.IsTrue( pgc.Values[1].IsSatisfied );
        }

        [TestMethod]
        [TestCategory( "PlayGridConstraints" )]
        public void IsSatisfied3()
        {
            var pgc = CreatePGConstraints( "x?x", 1, 1 );

            Assert.IsFalse( pgc.Values[0].IsSatisfied );
            Assert.IsFalse( pgc.Values[1].IsSatisfied );
        }

        [TestMethod]
        [TestCategory( "PlayGridConstraints" )]
        public void IsSatisfied4()
        {
            var pgc = CreatePGConstraints( "x?.x", 1, 1 );

            Assert.IsFalse( pgc.Values[0].IsSatisfied );
            Assert.IsTrue( pgc.Values[1].IsSatisfied );
        }

        [TestMethod]
        [TestCategory( "PlayGridConstraints" )]
        public void IsSatisfied5()
        {
            var pgc = CreatePGConstraints( "x.?.xx", 1, 2 );

            Assert.IsTrue( pgc.Values[0].IsSatisfied );
            Assert.IsTrue( pgc.Values[1].IsSatisfied );
        }

        [TestMethod]
        [TestCategory( "PlayGridConstraints" )]
        public void IsSatisfied6()
        {
            var pgc = CreatePGConstraints( "x.xxx.???.xx", 1, 3, 1, 2 );

            Assert.IsTrue( pgc.Values[0].IsSatisfied );
            Assert.IsTrue( pgc.Values[1].IsSatisfied );
            Assert.IsFalse( pgc.Values[2].IsSatisfied );
            Assert.IsTrue( pgc.Values[3].IsSatisfied );
        }

        private static Slice CreateSlice( string str )
        {
            return Slice.FromString( str );
        }

        private static Constraints CreateConstraints( params int[] constraints )
        {
            return new Constraints( constraints );
        }

        private static PlayGridConstraints CreatePGConstraints( string str, params int[] constraints )
        {
            return new PlayGridConstraints( CreateSlice( str ), CreateConstraints( constraints ) );
        }
    }
}
