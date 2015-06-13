using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PiCross.Game;

namespace PiCross.Tests
{
    [TestClass]
    public class Constraints_IsSatisfied
    {
        [TestMethod]
        [TestCategory( "Constraints" )]
        public void IsSatisfied1()
        {
            var slice = CreateSlice( "." );
            var constraints = CreateConstraints();

            Assert.IsTrue( constraints.IsSatisfied( slice ) );
        }

        [TestMethod]
        [TestCategory( "Constraints" )]
        public void IsSatisfied2()
        {
            var slice = CreateSlice( ".." );
            var constraints = CreateConstraints();

            Assert.IsTrue( constraints.IsSatisfied( slice ) );
        }

        [TestMethod]
        [TestCategory( "Constraints" )]
        public void IsSatisfied3()
        {
            var slice = CreateSlice( "x" );
            var constraints = CreateConstraints( 1 );

            Assert.IsTrue( constraints.IsSatisfied( slice ) );
        }

        [TestMethod]
        [TestCategory( "Constraints" )]
        public void IsSatisfied4()
        {
            var slice = CreateSlice( "." );
            var constraints = CreateConstraints( 1 );

            Assert.IsFalse( constraints.IsSatisfied( slice ) );
        }

        private static Slice CreateSlice( string str )
        {
            return Slice.FromString( str );
        }

        private static Constraints CreateConstraints( params int[] constraints )
        {
            return new Constraints( constraints );
        }
    }
}
