using Cells;
using DataStructures;

namespace PiCross
{
    public interface IPuzzleEditor
    {
        IGrid<IPuzzleEditorSquare> Grid { get; }

        ISequence<IPuzzleEditorConstraints> ColumnConstraints { get; }

        ISequence<IPuzzleEditorConstraints> RowConstraints { get; }

        void ResolveAmbiguity();

        Puzzle BuildPuzzle();
    }

    public interface IPuzzleEditorSquare
    {
        Cell<bool> IsFilled { get; }

        Cell<Ambiguity> Ambiguity { get; }

        Vector2D Position { get; }
    }

    public interface IPuzzleEditorConstraints
    {
        Cell<Constraints> Constraints { get; }
    }
}