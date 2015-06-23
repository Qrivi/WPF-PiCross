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
        Size Size { get; }

        IPuzzleEditorSquare this[Vector2D position] { get; }

        ISequence<IPuzzleEditorConstraints> ColumnConstraints { get; }

        ISequence<IPuzzleEditorConstraints> RowConstraints { get; }
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
