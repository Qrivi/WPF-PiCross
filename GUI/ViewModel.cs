using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Windows;
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

            Games = new List<GameViewModel>();
            foreach (var puzzle in puzzles)
                Games.Add(new GameViewModel(this, puzzle));

            Game = Cell.Create(Games[0]);
            PlayablePuzzle = Cell.Create(_model.CreateExtendedPlayablePuzzle(Puzzle.Value));
            Board = Cell.Create( new BoardViewModel(this));

            State.ValueChanged += StateChanged;
            Game.ValueChanged += GameChanged;

            NewGame = new NewGameCommand(this);
        }

        public Cell<GameState> State { get; }
        public IList<GameViewModel> Games { get; }
        public Cell<GameViewModel> Game { get; }
        public Cell<Puzzle> Puzzle => Game.Value.Puzzle;
        public Cell<IPlayablePuzzle> PlayablePuzzle { get; }
        public Cell<BoardViewModel> Board { get; }
        public Cell<TimeSpan> PlayTime => _chrono.TotalTime;
        public Cell<int> Mistakes => PlayablePuzzle.Value.Mistakes;
        public ICommand NewGame { get; }

        private void GameChanged()
        {
            PlayablePuzzle.Value = _model.CreateExtendedPlayablePuzzle(Puzzle.Value);
           // MessageBox.Show("Puzzle Width: " + Puzzle.Value.Grid.Size.Width +"\nPlayable Width: "+PlayablePuzzle.Value.Grid.Size.Width);
            //PlayablePuzzle.Refresh();
        }

        private void StateChanged()
        {
            switch (State.Value)
            {
                case GameState.Play:
                    Board.Value = new BoardViewModel(this);
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

        private bool CheckNewBestTime()
        {
            if (Game.Value.BestTime.Value != TimeSpan.Zero && Game.Value.BestTime.Value <= PlayTime.Value)
                return false;

            Game.Value.BestTime.Value = PlayTime.Value;
            return true;
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
                _viewModel.State.Value = GameState.Play;
            }

            public event EventHandler CanExecuteChanged;
        }
    }

    public sealed class GameViewModel
    {
        public GameViewModel(PiCrossViewModel piCrossViewModel, ExtendedPuzzle puzzle)
        {
            Name = puzzle.Name;
            Puzzle = puzzle.Puzzle;
            BestTime = puzzle.BestTime;

            SelectGame = new SelectGameCommand(piCrossViewModel, this);
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
                if (_piCrossVM.Game.Value == _gameVM)
                    _piCrossVM.State.Value = GameState.Setup;
                else
                    _piCrossVM.Game.Value = _gameVM;
            }

            public event EventHandler CanExecuteChanged;
        }
    }

    public sealed class BoardViewModel
    {
        private readonly Puzzle _puzzle;
        private readonly IPlayablePuzzle _playablePuzzle;

        public BoardViewModel(PiCrossViewModel piCrossViewModel)
        {
            _puzzle = piCrossViewModel.Puzzle.Value;
            _playablePuzzle = piCrossViewModel.PlayablePuzzle.Value;

            Grid = new List<BoardControlViewModel>();
            foreach (var square in _playablePuzzle.Grid.Items)
                Grid.Add(new BoardControlViewModel(piCrossViewModel, square));
        }

        public IList<BoardControlViewModel> Grid { get; }
        public IEnumerable<IPlayablePuzzleConstraints> RowConstraints => _playablePuzzle.RowConstraints.Items;
        public IEnumerable<IPlayablePuzzleConstraints> ColumnConstraints => _playablePuzzle.ColumnConstraints.Items;
        public int GridWidth => _playablePuzzle.Grid.Size.Width;
        public int GridHeight => _playablePuzzle.Grid.Size.Height;

        public bool CheckValidMove(Vector2D pos)
        {
            if (_puzzle.Grid[pos])
                return true;

            //_viewModel.AddMistake();
            return false;
        }

        public void CheckGameState()
        {
           // if (_playablePuzzle.IsSolved.Value)
              //  _viewModel.State.Value = GameState.Win;
        }
    }


    public sealed class BoardControlViewModel
    {
        private readonly PiCrossViewModel _piCrossVM;

        public BoardControlViewModel(PiCrossViewModel piCrossViewModel, IPlayablePuzzleSquare playablePuzzleSquare)
        {
            _piCrossVM = piCrossViewModel;
            PuzzleSquare = playablePuzzleSquare;

            PuzzleSquare.Contents.Value = Square.EMPTY;

            //CanClick = Cell.Derived<Square,bool>( Field.Contents, f => Equals(f, Square.EMPTY) );  // -> readonly 
            IsClicked = Cell.Create(false);

            Move = new MoveCommand(this);
        }

        public IPlayablePuzzleSquare PuzzleSquare { get; }
        public ICommand Move { get; }
        
        public Cell<bool> IsPlayable => _piCrossVM.PlayablePuzzle.Value.IsPlayable;
        public Cell<bool> IsClicked { get; }

        private void PerformMove()
        {
            if (_piCrossVM.Puzzle.Value.Grid[PuzzleSquare.Position])
            {
                PuzzleSquare.Contents.Value = Square.FILLED;
            }
            else
            {
                _piCrossVM.PlayablePuzzle.Value.Mistakes.Value++;
            }
               
            IsClicked.Value = true;
        }

        private class MoveCommand : ICommand
        {
            private readonly BoardControlViewModel boardControlVM;

            public MoveCommand(BoardControlViewModel boardControlViewModel)
            {
                boardControlVM = boardControlViewModel;
            }

            public bool CanExecute(object parameter)
            {
                return !boardControlVM.IsClicked.Value && boardControlVM.IsPlayable.Value;
            }

            public void Execute(object parameter)
            {
                boardControlVM.PerformMove();
            }

            public event EventHandler CanExecuteChanged;
        }
    }
}