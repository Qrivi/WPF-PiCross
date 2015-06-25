using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiCross.DataStructures;
using PiCross.Game;

namespace PiCross.Facade.IO
{
    public interface IPuzzleFormat
    {
        Puzzle Read( StreamReader reader );

        void Write( StreamWriter writer, Puzzle puzzle );
    }

    public class ReadableFormat : IPuzzleFormat
    {
        #region Reading

        public Puzzle Read( StreamReader reader )
        {
            var size = ReadPuzzleSize( reader );
            var grid = ReadPuzzleGrid( reader, size );

            return Puzzle.FromGrid( grid );
        }
        
        private Size ReadPuzzleSize( StreamReader reader )
        {
            var line = reader.ReadLine();
            var ns = line.Split( ' ' ).Select( int.Parse ).ToArray();

            var width = ns[0];
            var height = ns[1];

            return new Size( width, height );
        }

        private IGrid<Square> ReadPuzzleGrid( StreamReader reader, Size size )
        {
            var rows = ReadLines( reader, size.Height );

            return Grid.Create( size, position => rows[position.Y][position.X] ).Map( Square.FromSymbol );
        }

        private string[] ReadLines( StreamReader reader, int n )
        {
            var result = new string[n];

            for ( var i = 0; i != n; ++i )
            {
                result[i] = reader.ReadLine();
            }

            return result;
        }

        #endregion

        #region Writing

        public void Write( StreamWriter writer, Puzzle puzzle )
        {
            WritePuzzleSize( writer, puzzle.Size );
            WritePuzzleGrid( writer, puzzle.Grid );
        }

        private void WritePuzzleSize( StreamWriter writer, Size size )
        {
            writer.WriteLine( "{0} {1}", size.Width, size.Height );
        }

        private void WritePuzzleGrid( StreamWriter writer, IGrid<bool> grid )
        {
            foreach ( var row in grid.Rows )
            {
                WritePuzzleGridRow( writer, row );
            }
        }

        private void WritePuzzleGridRow( StreamWriter writer, ISequence<bool> row )
        {
            writer.WriteLine( row.Map( x => Square.FromBool( x ).Symbol ).Join() );
        }

        #endregion
    }

    public class CondensedFormat : IPuzzleFormat
    {
        public Puzzle Read( StreamReader reader )
        {
            var line = reader.ReadLine();
            var uid = PuzzleUID.FromBase64( line );

            return uid.CreatePuzzle();
        }

        public void Write( StreamWriter writer, Puzzle puzzle )
        {
            var uid = PuzzleUID.FromPuzzle( puzzle );

            writer.WriteLine( uid.Base64 );
        }
    }
}
