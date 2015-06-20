using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiCross.Cells;
using PiCross.DataStructures;
using PiCross.Game;

namespace PiCross.Facade.Editing
{
    public class PuzzleEditor : IPuzzleEditor
    {
        private readonly EditorGrid editorGrid;

        private readonly IGrid<PuzzleEditorSquare> facadeGrid;

        private readonly ISequence<PuzzleEditorColumnConstraints> columnConstraints;

        private readonly ISequence<PuzzleEditorRowConstraints> rowConstraints;

        public PuzzleEditor( EditorGrid grid )
        {
            if ( grid == null )
            {
                throw new ArgumentNullException( "grid" );
            }
            else
            {
                this.editorGrid = grid;
                facadeGrid = editorGrid.Contents.Map( position => new PuzzleEditorSquare( this, position ) ).Copy();
                columnConstraints = editorGrid.Contents.ColumnIndices.Select( x => new PuzzleEditorColumnConstraints( editorGrid, x ) ).ToSequence();
                rowConstraints = editorGrid.Contents.RowIndices.Select( y => new PuzzleEditorRowConstraints( editorGrid, y ) ).ToSequence();
            }
        }

        public int Width
        {
            get
            {
                return editorGrid.Width;
            }
        }

        public int Height
        {
            get
            {
                return editorGrid.Height;
            }
        }

        public IPuzzleEditorSquare this[Vector2D position]
        {
            get
            {
                return facadeGrid[position];
            }
        }

        public ISequence<IPuzzleEditorConstraints> ColumnConstraints
        {
            get
            {
                return this.columnConstraints;
            }
        }

        public ISequence<IPuzzleEditorConstraints> RowConstraints
        {
            get
            {
                return this.rowConstraints;
            }
        }

        private void Refresh( Vector2D position )
        {
            RefreshSquare( position );
            RefreshColumnConstraints( position.X );
            RefreshRowConstraints( position.Y );
        }

        private void RefreshSquare( Vector2D position )
        {
            this.facadeGrid[position].Refresh();
        }

        private void RefreshColumnConstraints( int x )
        {
            this.columnConstraints[x].Refresh();
        }

        private void RefreshRowConstraints( int x )
        {
            this.rowConstraints[x].Refresh();
        }

        private class PuzzleEditorSquare : IPuzzleEditorSquare
        {
            private readonly Cell<bool> contents;

            private readonly Vector2D position;

            public PuzzleEditorSquare( PuzzleEditor parent, Vector2D position )
            {
                this.contents = new PuzzleEditorSquareContentsCell( parent, position );
                this.position = position;
            }

            public Cell<bool> IsFilled
            {
                get
                {
                    return contents;
                }
            }

            public Vector2D Position
            {
                get
                {
                    return position;
                }
            }

            public void Refresh()
            {
                contents.Refresh();
            }
        }

        private class PuzzleEditorSquareContentsCell : ManualCell<bool>
        {
            private readonly PuzzleEditor parent;

            private readonly IVar<Square> contents;

            private readonly Vector2D position;

            public PuzzleEditorSquareContentsCell( PuzzleEditor parent, Vector2D position )
                : base( SquareToBool( parent.editorGrid.Squares[position] ) )
            {
                this.parent = parent;
                this.contents = parent.editorGrid.Contents[position];
                this.position = position;
            }

            private static bool SquareToBool(Square square)
            {
                return square == Square.FILLED;
            }

            private static Square BoolToSquare(bool b)
            {
                return b ? Square.FILLED : Square.EMPTY;
            }

            protected override bool ReadValue()
            {
                return SquareToBool( parent.editorGrid.Squares[position] );
            }

            protected override void WriteValue( bool value )
            {
                this.contents.Value = BoolToSquare( value );

                parent.Refresh( position );                
            }
        }

        private abstract class PuzzleEditorConstraints : IPuzzleEditorConstraints
        {
            private Cell<ISequence<int>> values;

            protected PuzzleEditorConstraints( Func<Constraints> constraintsFetcher )
            {
                values = Cell.Derived( () => constraintsFetcher().Values );
            }

            public Cell<ISequence<int>> Values
            {
                get
                {
                    return values;
                }
            }

            public void Refresh()
            {
                values.Refresh();
            }
        }

        private class PuzzleEditorRowConstraints : PuzzleEditorConstraints
        {
            public PuzzleEditorRowConstraints( EditorGrid parent, int row )
                : base( () => parent.DeriveRowConstraints( row ) )
            {
                // NOP
            }
        }

        private class PuzzleEditorColumnConstraints : PuzzleEditorConstraints
        {
            public PuzzleEditorColumnConstraints( EditorGrid parent, int column )
                : base( () => parent.DeriveColumnConstraints( column ) )
            {
                // NOP
            }
        }
    }
}
