using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiCross.DataStructures;
using PiCross.Facade.Solving;
using PiCross.Game;

namespace PiCross.Tests
{
    public class TestBase
    {
        protected static Slice CreateSlice( string str )
        {
            return Slice.FromString( str );
        }

        protected static Constraints CreateConstraints( params int[] constraints )
        {
            return new Constraints( constraints );
        }

        protected static ISequence<bool> CreateBooleans( string str )
        {
            return Sequence.FromString( str ).Map( c =>
            {
                switch ( c )
                {
                    case 't':
                        return true;

                    case 'f':
                        return false;

                    default:
                        throw new ArgumentException( "Invalid character" );
                }
            } );
        }

        protected static IGrid<Square> ParseFullyKnown( params string[] rows )
        {
            return Grid.CreateCharacterGrid( rows ).Map( c =>
            {
                switch ( c )
                {
                    case '.':
                    case ' ':
                        return Square.EMPTY;

                    case 'x':
                    case 'X':
                        return Square.FILLED;

                    default:
                        throw new ArgumentException( "Invalid character" );
                }
            } );
        }

        protected static IGrid<Square> ParsePartiallyKnown( params string[] rows )
        {
            return Grid.CreateCharacterGrid( rows ).Map( c =>
            {
                switch ( c )
                {
                    case 'x':
                    case '.':
                        return Square.UNKNOWN;

                    case 'X':
                        return Square.FILLED;

                    case ' ':
                        return Square.EMPTY;

                    default:
                        throw new ArgumentException( "Invalid character" );
                }
            } );
        }

        protected static EditorGrid ParseEditorGrid( params string[] rows )
        {
            var editorGridData = ParseFullyKnown( rows );

            return new EditorGrid( editorGridData );
        }

        protected static PlayGrid CreatePlayGrid( params string[] rows )
        {
            var editorGrid = ParseEditorGrid( rows );
            var playGrid = editorGrid.CreatePlayGrid();

            playGrid.Squares.Overwrite<Square>( ParsePartiallyKnown( rows ) );

            return playGrid;
        }

        protected static ObservablePlayGrid CreateManualPuzzle( params string[] rows )
        {
            var playGrid = CreatePlayGrid( rows );

            return new ObservablePlayGrid( playGrid );
        }
    }
}
