namespace PiCross
{
    internal class DummyData
    {
        public DummyData()
        {
            Puzzles = CreateDummyLibrary();
            Players = CreateDummyPlayerDatabase();
        }

        public InMemoryDatabase.PuzzleLibrary Puzzles { get; }

        public InMemoryDatabase.PlayerDatabase Players { get; }

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

        public static InMemoryDatabase Create()
        {
            var data = new DummyData();

            return new InMemoryDatabase(data.Puzzles, data.Players);
        }

        private static InMemoryDatabase.PlayerDatabase CreateDummyPlayerDatabase()
        {
            var db = InMemoryDatabase.PlayerDatabase.CreateEmpty();

            var woumpousse = db.CreateNewProfile("Woumpousse");
            var pimousse = db.CreateNewProfile("Pimousse");

            return db;
        }

        private static InMemoryDatabase.PuzzleLibrary CreateDummyLibrary()
        {
            var library = InMemoryDatabase.PuzzleLibrary.CreateEmpty();

            var author = "Woumpousse";

            library.Create(Puzzle1, author);
            library.Create(Puzzle2, author);
            library.Create(Puzzle3, author);
            library.Create(Puzzle4, author);
            library.Create(Puzzle5, author);
            library.Create(Puzzle6, author);

            return library;
        }
    }
}