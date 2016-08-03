using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml;
using Cells;
using DataStructures;
using PiCross;
using Utility;

namespace GUI
{
    public enum GameState
    {
        Init,
        Setup,
        Play,
        Win,
        Lose,
        BestTime
    }

    public sealed class PiCrossViewModel
    {
        private readonly Chronometer _chrono;
        private readonly DispatcherTimer _timer;

        public PiCrossViewModel()
        {
            _chrono = new Chronometer();
            _timer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(200)};

            State = Cell.Create(GameState.Init);
            ShowGames = Cell.Derived(State, s => s == GameState.Setup);
            ShowBoard = Cell.Derived(State,
                s => s == GameState.Play || s == GameState.Win || s == GameState.Lose || s == GameState.BestTime);
            Mistakes = Cell.Create(0);

            _timer.Tick += TimerTicked;
            State.ValueChanged += StateChanged;

            var puzzles = CreatePuzzles();
            Games = new List<GameViewModel>();
            foreach (var puzzle in puzzles)
                Games.Add(new GameViewModel(this, puzzle.Key, puzzle.Value));
            CurrentGame = Cell.Create(Games[0]);
            Board = Cell.Create(new BoardViewModel(this));

            NewGame = new NewGameCommand(this);
        }

        public IList<GameViewModel> Games { get; }
        public Cell<GameViewModel> CurrentGame { get; }
        public Cell<BoardViewModel> Board { get; }

        public Cell<GameState> State { get; }
        public Cell<bool> ShowGames { get; }
        public Cell<bool> ShowBoard { get; }
        public Cell<int> Mistakes { get; }
        public Cell<TimeSpan> PlayTime => _chrono.TotalTime;

        public ICommand NewGame { get; }

        public void AddMistake()
        {
            if (++Mistakes.Value == 5)
                State.Value = GameState.Lose;
        }

        private static Dictionary<string, Puzzle> CreatePuzzles()
        {
            var puzzles = new Dictionary<string, Puzzle>();
            var xml = new XmlDocument();
            xml.Load("Data/Puzzles.xml");

            var xmlRoot = xml.DocumentElement;
            var xmlPuzzles = xmlRoot?.SelectNodes("/puzzles/puzzle");

            if (xmlPuzzles == null)
                return puzzles;

            foreach (XmlNode xmlPuzzle in xmlPuzzles)
            {
                if (xmlPuzzle.Attributes == null)
                    continue;

                var name = xmlPuzzle.Attributes["name"].Value;
                var rows = new LinkedList<string>();

                var xmlPuzzleRows = xmlPuzzle.SelectNodes("rows/row");
                if (xmlPuzzleRows == null)
                    return puzzles;

                foreach (XmlNode xmlPuzzleRow in xmlPuzzleRows)
                    rows.AddLast(xmlPuzzleRow.InnerText);

                puzzles.Add(name, Puzzle.FromRowStrings(rows.ToArray()));
            }
            return puzzles;
        }

        private void TimerTicked(object sender, EventArgs e)
        {
            _chrono.Tick();
        }

        private void StateChanged()
        {
            switch (State.Value)
            {
                case GameState.Play:
                    Board.Value = new BoardViewModel(this);
                    Mistakes.Value = 0;
                    StartCounter();
                    break;
                case GameState.Win:
                    StopCounter();
                    if (CheckNewBestTime())
                        State.Value = GameState.BestTime;
                    break;
                case GameState.BestTime:
                case GameState.Lose:
                case GameState.Init:
                case GameState.Setup:
                    StopCounter();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void StartCounter()
        {
            _chrono.Reset();
            _chrono.Start();
            _timer.Start();
        }

        private void StopCounter()
        {
            _chrono.Tick();
            _chrono.Pause();
            _timer.Stop();
        }

        private bool CheckNewBestTime()
        {
            if (CurrentGame.Value.BestTime.Value != TimeSpan.Zero && CurrentGame.Value.BestTime.Value <= PlayTime.Value)
                return false;

            CurrentGame.Value.BestTime.Value = PlayTime.Value;
            return true;
        }

        private class NewGameCommand : ICommand
        {
            private readonly PiCrossViewModel _viewModel;

            public NewGameCommand(PiCrossViewModel viewModel)
            {
                _viewModel = viewModel;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                if (!CanSetGame())
                    return;
                _viewModel.State.Value = GameState.Init;
                _viewModel.State.Value = GameState.Play;
            }

            public event EventHandler CanExecuteChanged;

            private bool CanSetGame()
            {
                //if (_viewModel.State.Value != GameState.Play)
                return true;
            }
        }
    }

    public sealed class GameViewModel
    {
        public GameViewModel(PiCrossViewModel viewModel, string name, Puzzle puzzle, TimeSpan bestTime = new TimeSpan())
        {
            Name = Cell.Create(name);
            Puzzle = Cell.Create(puzzle);
            BestTime = Cell.Create(bestTime);
            SelectGame = new SelectGameCommand(viewModel, this);
        }

        public Cell<string> Name { get; }
        public Cell<Puzzle> Puzzle { get; }
        public Cell<TimeSpan> BestTime { get; }

        public ICommand SelectGame { get; }

        private class SelectGameCommand : ICommand
        {
            private readonly GameViewModel _current;

            private readonly PiCrossViewModel _viewModel;

            public SelectGameCommand(PiCrossViewModel viewModel, GameViewModel gameViewModel)
            {
                _viewModel = viewModel;
                _current = gameViewModel;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                if (!CanSetGame())
                    return;

                if (_viewModel.CurrentGame.Value == _current)
                    _viewModel.State.Value = GameState.Setup;
                else
                    _viewModel.CurrentGame.Value = _current;
            }

            public event EventHandler CanExecuteChanged;

            private bool CanSetGame()
            {
                // if (_viewModel.State.Value != GameState.Play)
                return true;
            }
        }
    }

    public sealed class BoardViewModel
    {
        private readonly PiCrossFacade _model;
        private readonly IPlayablePuzzle _playablePuzzle;
        private readonly Puzzle _puzzle;
        private readonly PiCrossViewModel _viewModel;

        public BoardViewModel(PiCrossViewModel piCrossViewModel)
        {
            _model = new PiCrossFacade();
            _viewModel = piCrossViewModel;
            _puzzle = _viewModel.CurrentGame.Value.Puzzle.Value;
            _playablePuzzle = _model.CreatePlayablePuzzle(_puzzle);

            EnableControls = Cell.Derived(_viewModel.State, s => s == GameState.Play);

            Grid = new List<BoardControlViewModel>();

            foreach (var square in _playablePuzzle.Grid.Items)
                Grid.Add(new BoardControlViewModel(this, square));
        }

        public IList<BoardControlViewModel> Grid { get; }
        public IEnumerable<IPlayablePuzzleConstraints> RowConstraints => _playablePuzzle.RowConstraints.Items;
        public IEnumerable<IPlayablePuzzleConstraints> ColumnConstraints => _playablePuzzle.ColumnConstraints.Items;

        public int GridWidth => _playablePuzzle.Grid.Size.Width;
        public int GridHeight => _playablePuzzle.Grid.Size.Height;

        public Cell<bool> EnableControls { get; }

        public bool CheckValidMove(Vector2D pos)
        {
            if (_puzzle.Grid[pos])
                return true;

            _viewModel.AddMistake();
            return false;
        }

        public void CheckGameState()
        {
            if (_playablePuzzle.IsSolved.Value)
                _viewModel.State.Value = GameState.Win;
        }
    }

    public sealed class BoardControlViewModel
    {
        private readonly BoardViewModel _boardViewModel;

        public BoardControlViewModel(BoardViewModel boardViewModel, IPlayablePuzzleSquare square)
        {
            _boardViewModel = boardViewModel;
            Field = square;

            Field.Contents.Value = Square.EMPTY;

            //CanClick = Cell.Derived<Square,bool>( Field.Contents, f => Equals(f, Square.EMPTY) );  // -> readonly 
            CanClick = Cell.Create(true);

            Move = new MoveCommand(this);
        }

        public IPlayablePuzzleSquare Field { get; }
        public ICommand Move { get; }

        public Cell<bool> CanPlay => _boardViewModel.EnableControls;
        public Cell<bool> CanClick { get; }

        private void PerformMove()
        {
            if (_boardViewModel.CheckValidMove(Field.Position))
            {
                Field.Contents.Value = Square.FILLED;
                _boardViewModel.CheckGameState();
            }
            CanClick.Value = false;
        }

        private class MoveCommand : ICommand
        {
            private readonly BoardControlViewModel _current;

            public MoveCommand(BoardControlViewModel viewModel)
            {
                _current = viewModel;
            }

            public bool CanExecute(object parameter)
            {
                return _current.CanClick.Value && _current.CanPlay.Value;
            }

            public void Execute(object parameter)
            {
                _current.PerformMove();
            }

            public event EventHandler CanExecuteChanged;
        }
    }
}