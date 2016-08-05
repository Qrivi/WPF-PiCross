using System;
using DataStructures;

namespace PiCross
{
    internal class StepwiseSolver : IStepwisePuzzleSolver
    {
        private readonly SolverGrid solverGrid;

        private SolveStep step;

        public StepwiseSolver(SolverGrid solverGrid)
        {
            this.solverGrid = solverGrid;
            step = new SolveStep(() => RefineColumn(0));
        }

        public void Step()
        {
            if (!solverGrid.IsSolved)
            {
                step = step.Perform();
            }
        }

        public IGrid<Square> Grid
        {
            get { return solverGrid.Squares; }
        }

        public bool IsSolved
        {
            get { return solverGrid.IsSolved; }
        }

        private SolveStep RefineRow(int row)
        {
            var nextStep = row + 1 == solverGrid.Height
                ? new SolveStep(() => RefineColumn(0))
                : new SolveStep(() => RefineRow(row + 1));

            if (solverGrid.RefineRow(row))
            {
                return nextStep;
            }
            return nextStep;
            // return nextStep.Perform();
        }

        private SolveStep RefineColumn(int column)
        {
            var nextStep = column + 1 == solverGrid.Width
                ? new SolveStep(() => RefineRow(0))
                : new SolveStep(() => RefineColumn(column + 1));

            if (solverGrid.RefineColumn(column))
            {
                return nextStep;
            }
            return nextStep.Perform();
        }

        private class SolveStep
        {
            private readonly Func<SolveStep> action;

            public SolveStep(Func<SolveStep> action)
            {
                this.action = action;
            }

            public SolveStep Perform()
            {
                return action();
            }
        }
    }
}