using Cells;
using DataStructures;

namespace PiCross
{
    /// <summary>
    ///     Represents a playable puzzle.
    ///     A playable puzzle offers the following functionality:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 A modifiable grid. Each square in the grid can take three values: UNKNOWN, FILLED and EMPTY (
    ///                 <see cref="Square" />.)
    ///                 The goal is for the player to fill in the grid correctly.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Row and colum constraints. These are all readonly.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </summary>
    public interface IPlayablePuzzle
    {
        /// <summary>
        ///     Grid of IPlayablePuzzleSquares.
        /// </summary>
        IGrid<IPlayablePuzzleSquare> Grid { get; }

        /// <summary>
        ///     Column constraints.
        /// </summary>
        ISequence<IPlayablePuzzleConstraints> ColumnConstraints { get; }

        /// <summary>
        ///     Row constraints.
        /// </summary>
        ISequence<IPlayablePuzzleConstraints> RowConstraints { get; }

        /// <summary>
        ///     Contains true if the Grid contains nothing but correct FILLED and EMPTY values,
        ///     false otherwise.
        /// </summary>
        Cell<bool> IsSolved { get; }

        Cell<bool> IsPlayable { get; }

        Cell<int> Mistakes { get; }
    }

    public interface IPlayablePuzzleSquare
    {
        /// <summary>
        ///     Contents of the square.
        /// </summary>
        Cell<Square> Contents { get; }

        /// <summary>
        ///     Position of the square in the grid.
        /// </summary>
        Vector2D Position { get; }
    }

    public interface IPlayablePuzzleConstraints
    {
        /// <summary>
        ///     Series of values.
        /// </summary>
        ISequence<IPlayablePuzzleConstraintsValue> Values { get; }

        /// <summary>
        ///     Contains true if the corresponding row or column satisfies
        ///     the pattern described by this object.
        /// </summary>
        Cell<bool> IsSatisfied { get; }
    }

    public interface IPlayablePuzzleConstraintsValue
    {
        /// <summary>
        ///     Actual value.
        /// </summary>
        int Value { get; }

        /// <summary>
        ///     Contains true if the corresponding row or column
        ///     satisfies this particular value.
        /// </summary>
        Cell<bool> IsSatisfied { get; }
    }
}