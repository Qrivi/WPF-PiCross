﻿using System;
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

        [TestMethod]
        [TestCategory("Sequence")]
        public void TakeWhile()
        {
            var seq = Sequence.FromItems( 1, 2, 3, 4, 5 );
            var actual = seq.TakeWhile( x => x < 4 );
            var expected = Sequence.FromItems( 1, 2, 3 );

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Sequence" )]
        public void DropWhile1()
        {
            var seq = Sequence.FromItems( 1, 2, 3, 4, 5 );
            var actual = seq.DropWhile( x => x < 4 );
            var expected = Sequence.FromItems( 4, 5 );

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Sequence" )]
        public void DropWhile2()
        {
            var seq = Sequence.FromItems( 1, 2, 3, 4, 5 );
            var actual = seq.DropWhile( x => x < 6 );
            var expected = Sequence.FromItems<int>();

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Sequence" )]
        public void Prefix()
        {
            var seq = Sequence.FromItems( 1, 2, 3, 4, 5 );
            var actual = seq.Prefix( 2 );
            var expected = Sequence.FromItems( 1, 2 );

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Sequence" )]
        public void Suffix()
        {
            var seq = Sequence.FromItems( 1, 2, 3, 4, 5 );
            var actual = seq.Suffix( 2 );
            var expected = Sequence.FromItems( 3, 4, 5 );

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Sequence" )]
        public void Reverse()
        {
            var seq = Sequence.FromItems( 1, 2, 3, 4, 5 );
            var actual = seq.Reverse();
            var expected = Sequence.FromItems( 5, 4, 3, 2, 1 );

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Sequence" )]
        public void Prefix_Full()
        {
            var seq = Sequence.FromItems( 1, 2, 3, 4, 5 );
            var expected = seq;
            var actual = seq.Prefix( 5 );

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Sequence" )]
        public void Suffix_Full()
        {
            var seq = Sequence.FromItems( 1, 2, 3, 4, 5 );
            var expected = seq;
            var actual = seq.Suffix( 0 );

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Sequence" )]
        public void Prefix_Empty()
        {
            var seq = Sequence.FromItems( 1, 2, 3, 4, 5 );
            var expected = Sequence.CreateEmpty<int>();
            var actual = seq.Prefix(0);

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Sequence" )]
        public void Suffix_Empty()
        {
            var seq = Sequence.FromItems( 1, 2, 3, 4, 5 );
            var expected = Sequence.CreateEmpty<int>();
            var actual = seq.Suffix( 5 );

            Assert.AreEqual( expected, actual );
        }
    }
}
