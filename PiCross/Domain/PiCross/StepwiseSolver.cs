﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiCross
{
    public class StepwiseSolver
    {
        private readonly SolverGrid solverGrid;

        private SolveStep step;

        public StepwiseSolver( SolverGrid solverGrid )
        {
            this.solverGrid = solverGrid;
            this.step = new SolveStep( () => RefineColumn( 0 ) );
        }

        public void Step()
        {
            if ( !solverGrid.IsSolved )
            {
                step = step.Perform();
            }
        }

        private SolveStep RefineRow( int row )
        {
            var nextStep = row + 1 == solverGrid.Height ? new SolveStep( () => RefineColumn( 0 ) ) : new SolveStep( () => RefineRow( row + 1 ) );

            if ( solverGrid.RefineRow( row ) )
            {
                return nextStep;
            }
            else
            {
                return nextStep;
                // return nextStep.Perform();
            }
        }

        private SolveStep RefineColumn( int column )
        {
            var nextStep = column + 1 == solverGrid.Width ? new SolveStep( () => RefineRow( 0 ) ) : new SolveStep( () => RefineColumn( column + 1 ) );

            if ( solverGrid.RefineColumn( column ) )
            {
                return nextStep;
            }
            else
            {
                return nextStep.Perform();
            }
        }

        public SolverGrid Grid
        {
            get
            {
                return solverGrid;
            }
        }

        private class SolveStep
        {
            private Func<SolveStep> action;

            public SolveStep( Func<SolveStep> action )
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
