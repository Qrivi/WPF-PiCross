using PiCross.DataStructures;
using PiCross.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiCross.Cells;

namespace PiCross.Facade.Solving
{
    public interface IPuzzle
    {
        int Width { get; }

        int Height { get; }

        IPuzzleSquare this[Vector2D position] { get; }

        ISequence<IPuzzleConstraints> ColumnConstraints { get; }

        ISequence<IPuzzleConstraints> RowConstraints { get; }
    }

    public interface IPuzzleSquare
    {
        Cell<Square> Contents { get; }
    }

    public interface IPuzzleConstraints
    {
        ISequence<IPuzzleConstraintsValue> Values { get; }

        Cell<bool> IsSatisfied { get; }
    }

    public interface IPuzzleConstraintsValue
    {
        int Value { get; }

        Cell<bool> IsSatisfied { get; }
    }
}
