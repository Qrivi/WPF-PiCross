using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using System.Xml;
using PiCross;
using Utility;

namespace GUI
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IChronometer _chrono;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _chrono = new Chronometer();

            var timer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(200)};
            var main = new MainWindow {DataContext = new PiCrossViewModel(CreatePuzzles(), _chrono)};

            timer.Tick += TimerTicked;

            timer.Start();
            main.Show();
        }

        private void TimerTicked(object sender, EventArgs e)
        {
            _chrono.Tick();
        }

        private static List<ExtendedPuzzle> CreatePuzzles()
        {
            var puzzles = new List<ExtendedPuzzle>();
            var xml = new XmlDocument();
            xml.Load("Data/Puzzles.xml");

            var xmlRoot = xml.DocumentElement;
            var xmlPuzzles = xmlRoot?.SelectNodes("/puzzles/puzzle");

            if (xmlPuzzles == null)
                return puzzles;

            foreach (XmlNode xmlPuzzle in xmlPuzzles)
            {
                var puzzle = new ExtendedPuzzle();

                if (xmlPuzzle.Attributes == null)
                    continue;

                var name = xmlPuzzle.Attributes["name"].Value;
                var rows = new LinkedList<string>();

                var xmlPuzzleRows = xmlPuzzle.SelectNodes("rows/row");
                if (xmlPuzzleRows == null)
                    return puzzles;

                foreach (XmlNode xmlPuzzleRow in xmlPuzzleRows)
                    rows.AddLast(xmlPuzzleRow.InnerText);

                puzzle.Name.Value = name;
                puzzle.Puzzle.Value = Puzzle.FromRowStrings(rows.ToArray());

                puzzles.Add(puzzle);
            }
            return puzzles;
        }
    }
}