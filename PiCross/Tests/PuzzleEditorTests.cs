using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PiCross.DataStructures;
using PiCross.Game;

namespace PiCross.Tests
{
    [TestClass]
    public class PuzzleEditorTests : TestBase
    {
        [TestMethod]
        [TestCategory("PuzzleEditor")]
        public void Initialization()
        {
            var editor = CreatePuzzleEditor( 
                "..x",
                "xxx",
                ".x."
                );

            Assert.AreEqual( 3, editor.Width );
            Assert.AreEqual( 3, editor.Height );

            Assert.AreEqual( Square.EMPTY, editor[new Vector2D( 0, 0 )].Contents.Value );
            Assert.AreEqual( Square.EMPTY, editor[new Vector2D( 1, 0 )].Contents.Value );
            Assert.AreEqual( Square.FILLED, editor[new Vector2D( 2, 0 )].Contents.Value );

            Assert.AreEqual( Square.FILLED, editor[new Vector2D( 0, 1 )].Contents.Value );
            Assert.AreEqual( Square.FILLED, editor[new Vector2D( 1, 1 )].Contents.Value );
            Assert.AreEqual( Square.FILLED, editor[new Vector2D( 2, 1 )].Contents.Value );

            Assert.AreEqual( Square.EMPTY, editor[new Vector2D( 0, 2 )].Contents.Value );
            Assert.AreEqual( Square.FILLED, editor[new Vector2D( 1, 2 )].Contents.Value );
            Assert.AreEqual( Square.EMPTY, editor[new Vector2D( 2, 2 )].Contents.Value );

            Assert.AreEqual( Sequence.FromItems( 1 ), editor.RowConstraints[0].Values.Value );
            Assert.AreEqual( Sequence.FromItems( 3 ), editor.RowConstraints[1].Values.Value );
            Assert.AreEqual( Sequence.FromItems( 1 ), editor.RowConstraints[2].Values.Value );

            Assert.AreEqual( Sequence.FromItems( 1 ), editor.ColumnConstraints[0].Values.Value );
            Assert.AreEqual( Sequence.FromItems( 2 ), editor.ColumnConstraints[1].Values.Value );
            Assert.AreEqual( Sequence.FromItems( 2 ), editor.ColumnConstraints[2].Values.Value );
        }
    }
}
