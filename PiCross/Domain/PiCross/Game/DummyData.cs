using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiCross.Game;
using PiCross.Facade;

namespace PiCross.Game
{
    public class DummyData
    {
        private readonly PuzzleLibrary library;

        private readonly PlayerDatabase players;

        public DummyData()
        {
            this.library = CreateDummyLibrary();
            this.players = CreateDummyPlayerDatabase();
        }

        public PuzzleLibrary Puzzles { get { return library; } }

        public PlayerDatabase Players { get { return players; } }

        private static PlayerDatabase CreateDummyPlayerDatabase()
        {
            var db = new PlayerDatabase();

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

        private static PuzzleLibrary CreateDummyLibrary()
        {
            var library = PuzzleLibrary.CreateEmpty();

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
