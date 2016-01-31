using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cells;
using DataStructures;
using PiCross.Game;

namespace PiCross.Facade.Editing
{
    public interface IPuzzleEditor
    {
        IGrid<IPuzzleEditorSquare> Grid { get; }

        ISequence<IPuzzleEditorConstraints> ColumnConstraints { get; }

        ISequence<IPuzzleEditorConstraints> RowConstraints { get; }

        void ResolveAmbiguity();
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
