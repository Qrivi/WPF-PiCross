using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PiCross.Controls;
using DataStructures;
using PiCross;

namespace ControlsDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var puzzle = Puzzle.FromRowStrings(
                "xxxxx",
                "x...x",
                "x...x",
                "x...x",
                "xxxxx" );
            this.DataContext = new PuzzleData( puzzle );
        }
    }

    public class PuzzleData : IPuzzleData
    {
        private readonly Puzzle puzzle;

        public PuzzleData( Puzzle puzzle )
        {
            this.puzzle = puzzle;
        }

        public IGrid<object> Grid
        {
            get
            {
                // Needed because IGrid<bool> is not a subtype of IGrid<object>
                // So I create a new grid where every bool in the original is
                // converted to an object (i.e. boxing)
                return puzzle.Grid.Map<bool, object>( (bool b) => b );
            }
        }

        public ISequence<object> ColumnConstraints
        {
            get
            {
                return puzzle.ColumnConstraints;
            }
        }

        public ISequence<object> RowConstraints
        {
            get { return puzzle.RowConstraints; }
        }
    }
}
