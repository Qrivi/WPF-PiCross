using PiCross.DataStructures;
using PiCross.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiCross.Facade.Solving
{
    public interface IPuzzle
    {
        int Width { get; }

        int Height { get; }

        IPuzzleSquare this[Vector2D position] { get; }

        IPuzzleConstraints ColumnConstraints( int x );

        IPuzzleConstraints RowConstraints( int y );
    }

    public interface IPuzzleSquare
    {
        ICell<Square> Contents { get; }
    }

    public interface IPuzzleConstraints
    {
        ISequence<IPuzzleConstraintsValue> Constraints { get; }

        ICell<bool> IsSatisfied { get; }
    }

    public interface IPuzzleConstraintsValue
    {
        int Value { get; }

        ICell<bool> IsSatisfied { get; }
    }
}
