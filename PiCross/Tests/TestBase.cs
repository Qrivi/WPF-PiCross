using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiCross.DataStructures;
using PiCross.Facade.Editing;
using PiCross.Facade.IO;
using PiCross.Facade.Playing;
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
            return Sequence.FromString( str ).Map( CharToBool );
        }

        protected static IGrid<bool> CreateBooleansGrid( params string[] str )
        {
            return Grid.CreateCharacterGrid( str ).Map( CharToBool );
        }

        private static bool CharToBool(char c)
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

        protected static PiCross.Facade.Playing.PlayablePuzzleImplementation CreateManualPuzzle( params string[] rows )
        {
            var playGrid = CreatePlayGrid( rows );

            return new PiCross.Facade.Playing.PlayablePuzzleImplementation( playGrid );
        }

        protected static void OverwritePlayGrid( PlayGrid grid, params string[] rows)
        {
            grid.Squares.Overwrite( Square.CreateGrid( rows ) );
        }

        protected static IPuzzleEditor CreatePuzzleEditor(params string[] rows)
        {
            var editorGrid = EditorGrid.FromStrings( rows );

            return new PuzzleEditor_ManualAmbiguity( editorGrid );
        }

        protected static PiCross.Game.Puzzle CreatePuzzle(params string[] rows)
        {
            return PiCross.Game.Puzzle.FromRowStrings( rows );
        }

        protected static SolverGrid CreateSolverGrid(params string[] rows)
        {
            var editorGrid = EditorGrid.FromStrings( rows );

            return editorGrid.CreateSolverGrid();
        }

        protected static AmbiguityChecker CreateAmbiguityChecker(params string[] rows)
        {
            var puzzle = PiCross.Game.Puzzle.FromRowStrings( rows );

            return new AmbiguityChecker( columnConstraints: puzzle.ColumnConstraints, rowConstraints: puzzle.RowConstraints );
        }

        protected static ILibrary CreateLibrary(params PiCross.Game.Puzzle[] puzzles)
        {
            var author = "test";
            var library = Library.CreateEmpty();

            foreach ( var puzzle in puzzles )
            {
                var entry = new LibraryEntry( puzzle, author );

                library.Entries.Add( entry );
            }

            return library;
        }
    }
}
