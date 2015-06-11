using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PiCross.DataStructures;
using PiCross.Game;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    [TestClass]
    public class IntegerGenerationTests
    {
        [TestMethod]
        public void Test1()
        {
            var test = new Test( length: 0, sum: 0 );

            test.Check();

            test.Done();
        }

        [TestMethod]
        public void Test2()
        {
            var test = new Test( length: 1, sum: 0 );

            test.Check( 0 );
            test.Done();
        }

        [TestMethod]
        public void Test3()
        {
            var test = new Test( length: 1, sum: 1 );

            test.Check( 1 );
            test.Done();
        }

        [TestMethod]
        public void Test4()
        {
            var test = new Test( length: 2, sum: 3 );

            test.Check( 0, 3 );
            test.Check( 1, 2 );
            test.Check( 2, 1 );
            test.Check( 3, 0 );

            test.Done();
        }

        [TestMethod]
        public void Test5()
        {
            var test = new Test( length: 3, sum: 2 );

            test.Check( 0, 0, 2 );
            test.Check( 0, 2, 0 );
            test.Check( 2, 0, 0 );

            test.Check( 0, 1, 1 );
            test.Check( 1, 0, 1 );
            test.Check( 1, 1, 0 );

            test.Done();
        }

        private class Test
        {
            private readonly List<ISequence<int>> sequences;

            public Test(int length, int sum)
            {
                this.sequences = Solver.GenerateIntegers( length, sum ).ToList();
            }

            public void Check(params int[] ns)
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
