using DataStructures;

namespace PiCross
{
    public interface IStepwisePuzzleSolver
    {
        IGrid<Square> Grid { get; }

        bool IsSolved { get; }

        void Step();
    }
}