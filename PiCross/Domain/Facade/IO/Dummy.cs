using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiCross.Game;

namespace PiCross.Facade.IO
{
    public class Dummy
    {
        private readonly ILibrary library;

        private readonly IUserDatabase users;

        public Dummy()
        {
            this.library = CreateDummyLibrary();
            this.users = CreateDummyUserDatabase();
        }

        private static IUserDatabase CreateDummyUserDatabase()
        {
            var db = new UserDatabase();

            var woumpousse = db.CreateNewProfile( "Woumpousse" );
            var pimousse = db.CreateNewProfile( "Pimousse" );

            woumpousse.PuzzleInformation[Puzzle1] = new UserPuzzleInformationEntry( TimeSpan.FromSeconds( 5 ) );

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
