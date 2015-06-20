using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PiCross.Cells;
using PiCross.DataStructures;

namespace PiCross.Tests
{
    [TestClass]
    public class CellTests
    {
        [TestMethod]
        [TestCategory("Cell")]
        public void Cell_EventFiredWhenValueChange()
        {
            var cell = CreateCell( 0 );
            var flag = Flag.Create( cell );

            Assert.IsFalse( flag.Status );
            cell.Value = 1;
            Assert.IsTrue( flag.Status );
        }

        [TestMethod]
        [TestCategory( "Cell" )]
        public void Cell_NoEventFiredWhenValueChangeToSameOldValue()
        {
            var cell = CreateCell( 0 );
            var flag = Flag.Create( cell );

            Assert.IsFalse( flag.Status );
            cell.Value = 0;
            Assert.IsFalse( flag.Status );
        }

        [TestMethod]
        [TestCategory( "Cell" )]
        public void Derived_Initialized()
        {
            var cell = CreateCell( 2 );
            var derived = CreateDerived( cell, x => x * x );

            Assert.AreEqual( 4, derived.Value );
        }

        [TestMethod]
        [TestCategory( "Cell" )]
        public void Derived_ChangesWithCell()
        {
            var cell = CreateCell( 2 );
            var derived = CreateDerived( cell, x => x * x );

            Assert.AreEqual( 4, derived.Value );
            cell.Value = 5;
            Assert.AreEqual( 25, derived.Value );
        }

        private static Cell<T> CreateCell<T>(T value)
        {
            return Cell.Create<T>( value );
        }

        private static Cell<R> CreateDerived<T, R>( Cell<T> cell, Func<T, R> function)
        {
            return Cell.Derived( cell, function );
        }
    }
}
