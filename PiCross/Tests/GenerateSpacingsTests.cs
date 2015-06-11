using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using PiCross.Game;
using PiCross.DataStructures;

namespace PiCross.Tests
{
    [TestClass]
    public class GenerateSpacingsTests
    {
        [TestMethod]
        public void Length_5_NoConstraints()
        {
            var test = new Test( 5 );

            test.Check( 5 );

            test.Done();
        }

        [TestMethod]
        public void Length_5_Constraints_1()
        {
            var test = new Test( 5, 1 );

            test.Check( 0, 4 );
            test.Check( 1, 3 );
            test.Check( 2, 2 );
            test.Check( 3, 1 );
            test.Check( 4, 0 );

            test.Done();
        }

        [TestMethod]
        public void Length_5_Constraints_2()
        {
            var test = new Test( 5, 2 );

            test.Check( 0, 3 );
            test.Check( 1, 2 );
            test.Check( 2, 1 );
            test.Check( 3, 0 );

            test.Done();
        }

        [TestMethod]
        public void Length_3_Constraints_1_1()
        {
            var test = new Test( 3, 1, 1 );

            test.Check( 0, 1, 0 );

            test.Done();
        }

        private class Test
        {
            private readonly List<ISequence<int>> sequences;

            public Test( int length, params int[] constraints )
            {
                var constraintCount = constraints.Length;
                var constraintSum = constraints.Sum();

                this.sequences = Solver.GenerateSpacings( length, constraintCount: constraintCount, constraintSum: constraintSum ).ToList();
            }

            public void Check( params int[] ns )
            {
                var seq = Sequence.FromItems( ns );

                Assert.IsTrue( sequences.Contains( seq ), "{0} should appear", seq );
                sequences.Remove( seq );
            }

            public void Done()
            {
                Assert.AreEqual( 0, sequences.Count );
            }
        }
    }
}
