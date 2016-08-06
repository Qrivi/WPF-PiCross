﻿using System;
using System.Collections.Generic;
using System.Linq;
using DataStructures;

namespace PiCross
{
    internal class AmbiguityChecker
    {
        private readonly SolverGrid solver;

        private readonly IEnumerator<bool> stepwiseFunction;

        public AmbiguityChecker(ISequence<Constraints> columnConstraints, ISequence<Constraints> rowConstraints)
        {
            solver = new SolverGrid(columnConstraints, rowConstraints);
            stepwiseFunction = CreateStepwiseFunction().GetEnumerator();
            Ambiguities = solver.Squares.Map(DeriveAmbiguity);
        }

        public bool IsAmbiguityResolved
        {
            get { return stepwiseFunction.Current; }
        }

        public IGrid<Ambiguity> Ambiguities { get; }

        public bool IsAmbiguous
        {
            get
            {
                if (!IsAmbiguityResolved)
                {
                    throw new InvalidOperationException("Ambiguity not resolved yet; use Resolve() first");
                }
                return Ambiguities.Items.Any(a => a == Ambiguity.Ambiguous);
            }
        }

        private Ambiguity DeriveAmbiguity(Square square)
        {
            if (square == Square.UNKNOWN)
            {
                if (IsAmbiguityResolved)
                {
                    return Ambiguity.Ambiguous;
                }
                return Ambiguity.Unknown;
            }
            return Ambiguity.Unambiguous;
        }

        private IEnumerable<bool> CreateStepwiseFunction()
        {
            bool changeDetected;

            do
            {
                changeDetected = false;

                foreach (var x in solver.Squares.ColumnIndices)
                {
                    changeDetected = solver.RefineColumn(x) || changeDetected;

                    yield return false;
                }

                foreach (var y in solver.Squares.RowIndices)
                {
                    changeDetected = solver.RefineRow(y) || changeDetected;

                    yield return false;
                }
            } while (changeDetected);

            yield return true;
        }

        public void Step()
        {
            if (!stepwiseFunction.Current)
            {
                stepwiseFunction.MoveNext();
            }
        }

        public void Resolve()
        {
            while (!IsAmbiguityResolved)
            {
                Step();
            }
        }
    }
}