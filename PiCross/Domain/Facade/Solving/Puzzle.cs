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

        IPuzzleConstraints RowConstraints( int x );
    }

    public interface IPuzzleSquare
    {
        ICell<Square> Contents { get; }
    }

    public interface IPuzzleConstraints
    {
        ISequence<IPuzzleConstraint> Constraints { get; }

        ICell<bool> IsSatisfied { get; }
    }

    public interface IPuzzleConstraint
    {
        int Value { get; }

        ICell<bool> IsSatisfied { get; }
    }
}
