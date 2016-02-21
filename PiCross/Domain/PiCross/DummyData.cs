using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiCross;

namespace PiCross
{
    internal class DummyData
    {
        private readonly InMemoryDatabase.PuzzleLibrary library;

        private readonly InMemoryPlayerDatabase players;

        public static InMemoryDatabase Create()
        {
            var data = new DummyData();

            return new InMemoryDatabase( data.library, data.players );
        }

        public DummyData()
        {
            this.library = CreateDummyLibrary();
            this.players = CreateDummyPlayerDatabase();
        }

        public InMemoryDatabase.PuzzleLibrary Puzzles { get { return library; } }

        public InMemoryPlayerDatabase Players { get { return players; } }

        private static InMemoryPlayerDatabase CreateDummyPlayerDatabase()
        {
            var db = InMemoryPlayerDatabase.CreateEmpty();

            var woumpousse = db.CreateNewProfile( "Woumpousse" );
            var pimousse = db.CreateNewProfile( "Pimousse" );

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

        private static InMemoryDatabase.PuzzleLibrary CreateDummyLibrary()
        {
            var library = InMemoryDatabase.PuzzleLibrary.CreateEmpty();

            var author = "Woumpousse";

            library.Create( Puzzle1, author );
            library.Create( Puzzle2, author );
            library.Create( Puzzle3, author );
            library.Create( Puzzle4, author );
            library.Create( Puzzle5, author );
            library.Create( Puzzle6, author );

            return library;
        }
    }
}
