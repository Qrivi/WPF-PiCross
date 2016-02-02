using System;
using System.Collections.Generic;
using GUI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataStructures;

namespace PiCross.Tests
{
    [TestClass]
    public class GridSelectionHelperTests
    {
        [TestMethod]
        [TestCategory( "GridSelectionHelper" )]
        public void Initialization()
        {
            var helper = new GridSelectionHelper( new Size( 5, 5 ) );

            CheckGrid( helper );
        }

        [TestMethod]
        [TestCategory( "GridSelectionHelper" )]
        public void PerfectSelection()
        {
            var helper = new GridSelectionHelper( new Size( 5, 5 ) );

            helper.SelectionStart = new Vector2D( 0, 0 );
            helper.SelectionEnd = new Vector2D( 0, 0 );

            CheckGrid( helper, new Vector2D( 0, 0 ) );
        }

        [TestMethod]
        [TestCategory( "GridSelectionHelper" )]
        public void PerfectHorizontalSelection()
        {
            var helper = new GridSelectionHelper( new Size( 5, 5 ) );

            helper.SelectionStart = new Vector2D( 1, 1 );
            helper.SelectionEnd = new Vector2D( 3, 1 );

            CheckGrid( helper, new Vector2D( 1, 1 ), new Vector2D( 2, 1 ), new Vector2D( 3, 1 ) );
        }

        [TestMethod]
        [TestCategory( "GridSelectionHelper" )]
        public void ImperfectHorizontalSelection()
        {
            var helper = new GridSelectionHelper( new Size( 5, 5 ) );

            helper.SelectionStart = new Vector2D( 1, 1 );
            helper.SelectionEnd = new Vector2D( 3, 2 );

            CheckGrid( helper, new Vector2D( 1, 1 ), new Vector2D( 2, 1 ), new Vector2D( 3, 1 ) );
        }

        [TestMethod]
        [TestCategory( "GridSelectionHelper" )]
        public void ImperfectHorizontalSelection_RightToLeft()
        {
            var helper = new GridSelectionHelper( new Size( 5, 5 ) );

            helper.SelectionStart = new Vector2D( 3, 1 );
            helper.SelectionEnd = new Vector2D( 1, 2 );

            CheckGrid( helper, new Vector2D( 3, 1 ), new Vector2D( 2, 1 ), new Vector2D( 1, 1 ) );
        }

        [TestMethod]
        [TestCategory( "GridSelectionHelper" )]
        public void PerfectVerticalSelection()
        {
            var helper = new GridSelectionHelper( new Size( 5, 5 ) );

            helper.SelectionStart = new Vector2D( 2, 0 );
            helper.SelectionEnd = new Vector2D( 2, 4 );

            CheckGrid( helper, new Vector2D( 2, 0 ), new Vector2D( 2, 1 ), new Vector2D( 2, 2 ), new Vector2D( 2, 3 ), new Vector2D( 2, 4 ) );
        }

        [TestMethod]
        [TestCategory( "GridSelectionHelper" )]
        public void ImperfectVerticalSelection()
        {
            var helper = new GridSelectionHelper( new Size( 5, 5 ) );

            helper.SelectionStart = new Vector2D( 2, 0 );
            helper.SelectionEnd = new Vector2D( 4, 4 );

            CheckGrid( helper, new Vector2D( 2, 0 ), new Vector2D( 2, 1 ), new Vector2D( 2, 2 ), new Vector2D( 2, 3 ), new Vector2D( 2, 4 ) );
        }

        private void CheckGrid( GridSelectionHelper helper, params Vector2D[] expected )
        {
            var set = new HashSet<Vector2D>( expected );

            for ( var i = 0; i != expected.Length; ++i )
            {
                var p = expected[i];

                Assert.IsTrue( helper.Selection[p].Value.HasValue, string.Format( "Position {0} should contain a number", p ) );
                Assert.AreEqual( i, helper.Selection[p].Value.Value, string.Format( "Position {0} should contain {1}", p, i ) );
            }

            foreach ( var p in helper.Selection.AllPositions )
            {
                if ( !set.Contains( p ) )
                {
                    Assert.IsFalse( helper.Selection[p].Value.HasValue, string.Format( "Position {0} should not contain a number", p ) );
                }
            }
        }
    }
}
