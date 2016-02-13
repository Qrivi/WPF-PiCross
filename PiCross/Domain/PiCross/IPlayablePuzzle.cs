using DataStructures;
using PiCross;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cells;

namespace PiCross
{
    public interface IPlayablePuzzle
    {
        IGrid<IPlayablePuzzleSquare> Grid { get; }

        ISequence<IPlayablePuzzleConstraints> ColumnConstraints { get; }

        ISequence<IPlayablePuzzleConstraints> RowConstraints { get; }

        Cell<bool> IsSolved { get; }
    }

    public interface IPlayablePuzzleSquare
    {
        Cell<Square> Contents { get; }

        Vector2D Position { get; }
    }

    public interface IPlayablePuzzleConstraints
    {
        ISequence<IPlayablePuzzleConstraintsValue> Values { get; }

        Cell<bool> IsSatisfied { get; }
    }

    public interface IPlayablePuzzleConstraintsValue
    {
        int Value { get; }

        Cell<bool> IsSatisfied { get; }
    }

    public static class PlayablePuzzle
    {
        public static IPlayablePuzzle Create(Puzzle puzzle)
        {
            return new PlayablePuzzleImplementation( columnConstraints: puzzle.ColumnConstraints, rowConstraints: puzzle.RowConstraints );
        }
    }
}
