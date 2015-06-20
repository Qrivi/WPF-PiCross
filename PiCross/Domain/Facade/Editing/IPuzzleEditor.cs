using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiCross.Cells;
using PiCross.DataStructures;
using PiCross.Game;

namespace PiCross.Facade.Editing
{
    public interface IPuzzleEditor
    {
        int Width { get; }

        int Height { get; }

        IPuzzleEditorSquare this[Vector2D position] { get; }

        ISequence<IPuzzleEditorConstraints> ColumnConstraints { get; }

        ISequence<IPuzzleEditorConstraints> RowConstraints { get; }
    }

    public interface IPuzzleEditorSquare
    {
        Cell<Square> Contents { get; }

        Vector2D Position { get; }
    }

    public interface IPuzzleEditorConstraints
    {
        Cell<ISequence<int>> Values { get; }
    }
}
