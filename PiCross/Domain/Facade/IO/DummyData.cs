using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiCross.Game;

namespace PiCross.Facade.IO
{
    public class DummyData
    {
        private readonly ILibrary library;

        private readonly IPlayerDatabase players;

        public DummyData()
        {
            this.library = CreateDummyLibrary();
            this.players = CreateDummyPlayerDatabase();
        }

        public ILibrary Puzzles { get { return library; } }

        public IPlayerDatabase Players { get { return players; } }

        private static IPlayerDatabase CreateDummyPlayerDatabase()
        {
            var db = new PlayerDatabase();

            var woumpousse = db.CreateNewProfile( "Woumpousse" );
            var pimousse = db.CreateNewProfile( "Pimousse" );

            woumpousse.PuzzleInformation[Puzzle1].BestTime.Value = TimeSpan.FromSeconds( 5 );

            return db;
        }

        private static Puzzle Puzzle1
        {
            get
            {
                return Puzzle.FromRowStrings(
                    "..x..",
                    ".x.x.",
                    "x...x",
                    ".x.x.",
                    "..x.."
                    );
            }
        }

        private static Puzzle Puzzle2
        {
            get
            {
                return Puzzle.FromRowStrings(
                    "x...x",
                    ".x.x.",
                    "..x..",
                    ".x.x.",
                    "x...x"
                    );
            }
        }

        private static Puzzle Puzzle3
        {
            get
            {
                return Puzzle.FromRowStrings(
                    ".x..x",
                    ".....",
                    "..x..",
                    "x...x",
                    ".xxx."
                    );
            }
        }

        private static Puzzle Puzzle4
        {
            get
            {
                return Puzzle.FromRowStrings(
                    "..x..",
                    ".xxx.",
                    "xxxxx",
                    ".xxx.",
                    "..x.."
                    );
            }
        }

        private static Puzzle Puzzle5
        {
            get
            {
                return Puzzle.FromRowStrings(
                    "..........",
                    "..........",
                    "..........",
                    "..........",
                    "..........",
                    "..........",
                    "..........",
                    "..........",
                    "..........",
                    ".........."
                    );
            }
        }

        private static Puzzle Puzzle6
        {
            get
            {
                return Puzzle.FromRowStrings(
                    "..........",
                    ".xxx..xxx.",
                    "...xx...x.",
                    ".xxx....x.",
                    "..xxxx....",
                    ".....xxxx.",
                    "..xxxx....",
                    "....xxxxx.",
                    "....x...x.",
                    "....x....."
                    );
            }
        }

        private static ILibrary CreateDummyLibrary()
        {
            var library = Library.CreateEmpty();

            var author = "Woumpousse";
            library.Entries.Add( new LibraryEntry( Puzzle1, author ) );
            library.Entries.Add( new LibraryEntry( Puzzle2, author ) );
            library.Entries.Add( new LibraryEntry( Puzzle3, author ) );
            library.Entries.Add( new LibraryEntry( Puzzle4, author ) );
            library.Entries.Add( new LibraryEntry( Puzzle5, author ) );
            library.Entries.Add( new LibraryEntry( Puzzle6, author ) );            

            return library;
        }
    }
}
