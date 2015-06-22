using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiCross.Game;

namespace PiCross.Facade.Editing
{
    public static class PuzzleEditor
    {
        public static IPuzzleEditor Create(EditorGrid editorGrid)
        {
            return new PuzzleEditor_ManualAmbiguity( editorGrid );
        }
    }
}
