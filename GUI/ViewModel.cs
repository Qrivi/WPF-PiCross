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
        private readonly PiCrossFacade _model;
        private readonly Chronometer _chrono;

        public PiCrossViewModel( List<ExtendedPuzzle> puzzles , Chronometer chronometer)
        {
            _model = new PiCrossFacade();
            _chrono = chronometer;
           
            State = Cell.Create(GameState.Init);
            State.ValueChanged += StateChanged;

            Games = new List<GameViewModel>();
            foreach (var puzzle in puzzles)
                Games.Add(new GameViewModel(this, puzzle));
            CurrentGame = Cell.Create(Games[0]);

            Board = Cell.Create(new BoardViewModel(this, _model));

            NewGame = new NewGameCommand(this);
        }

        public Cell<GameState> State { get; }
        public IList<GameViewModel> Games { get; }
        public Cell<GameViewModel> CurrentGame { get; }
        public Cell<BoardViewModel> Board { get; }

        public Cell<TimeSpan> PlayTime => _chrono.TotalTime;

        public ICommand NewGame { get; }

        private void StateChanged()
        {
            switch (State.Value)
            {
                case GameState.Play:
                    Board.Value = new BoardViewModel(this, _model);
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
        }

        private void StopCounter()
        {
            _chrono.Tick();
            _chrono.Pause();
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
        public GameViewModel(PiCrossViewModel viewModel, ExtendedPuzzle puzzle)
        {
            Name = puzzle.Name;
            Puzzle = puzzle.Puzzle;
            BestTime = puzzle.BestTime;

            SelectGame = new SelectGameCommand(viewModel, this);
        }

        public Cell<string> Name { get; }
        public Cell<Puzzle> Puzzle { get; }
        public Cell<TimeSpan> BestTime { get; }

        public ICommand SelectGame { get; }

        private class SelectGameCommand : ICommand
        {
            private readonly GameViewModel _gameVM;
            private readonly PiCrossViewModel _piCrossVM;

            public SelectGameCommand(PiCrossViewModel piCrossViewModel, GameViewModel gameViewModel)
            {
                _piCrossVM = piCrossViewModel;
                _gameVM = gameViewModel;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                if (_piCrossVM.CurrentGame.Value == _gameVM)
                    _piCrossVM.State.Value = GameState.Setup;
                else
                    _piCrossVM.CurrentGame.Value = _gameVM;
            }

            public event EventHandler CanExecuteChanged;
        }
    }

    public sealed class BoardViewModel
    {
        private readonly PiCrossViewModel _piCrossVM;

        private readonly IPlayablePuzzle _playablePuzzle;
        private readonly Puzzle _puzzle;

        public BoardViewModel(PiCrossViewModel piCrossViewModel, PiCrossFacade piCrossFacade)
        {
            _piCrossVM = piCrossViewModel;
            _puzzle = _piCrossVM.CurrentGame.Value.Puzzle.Value;
            _playablePuzzle = piCrossFacade.CreatePlayablePuzzle(_puzzle);

            EnableControls = Cell.Derived(_piCrossVM.State, s => s == GameState.Play);

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

            //_viewModel.AddMistake();
            return false;
        }

        public void CheckGameState()
        {
            if (_playablePuzzle.IsSolved.Value)
                _piCrossVM.State.Value = GameState.Win;
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