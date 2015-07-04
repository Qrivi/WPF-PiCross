using PiCross.DataStructures;
using PiCross.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiCross.Cells;

namespace PiCross.Facade.Playing
{
    public interface IPlayablePuzzle
    {
        Size Size { get; }

        IPlayablePuzzleSquare this[Vector2D position] { get; }

        ISequence<IPlayablePuzzleConstraints> ColumnConstraints { get; }

        ISequence<IPlayablePuzzleConstraints> RowConstraints { get; }
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
}
