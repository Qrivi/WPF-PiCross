using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DataStructures;

namespace PiCross
{
    public sealed class Constraints
    {
        private Constraints(ISequence<int> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            if (values.Items.Any(n => n <= 0))
            {
                throw new ArgumentOutOfRangeException("values must all be strictly positive");
            }
            Values = values;
        }

        private Constraints(params int[] values)
            : this(Sequence.FromItems(values))
        {
            // NOP
        }

        private Constraints(IEnumerable<int> values)
            : this(values.ToArray())
        {
            // NOP
        }

        public ISequence<int> Values { get; }

        public static Constraints FromValues(params int[] values)
        {
            return new Constraints(values.ToSequence());
        }

        public static Constraints FromValues(IEnumerable<int> values)
        {
            return new Constraints(values);
        }

        public static Constraints FromSequence(ISequence<int> sequence)
        {
            return new Constraints(sequence);
        }

        internal IEnumerable<Slice> GenerateSlices(int sliceLength)
        {
            return GeneratePatterns(sliceLength, Values).Select(x => new Slice(x));
        }

        private static IEnumerable<ISequence<int>> GenerateIntegers(int count, int sum)
        {
            Debug.Assert(count >= 0);
            Debug.Assert(sum >= 0);

            if (count == 0)
            {
                if (sum == 0)
                {
                    return new List<ISequence<int>> {Sequence.CreateEmpty<int>()};
                }
                return Enumerable.Empty<ISequence<int>>();
            }
            // TODO Possible optimization: create separate sequence prefixer class
            return from i in Enumerable.Range(0, sum + 1)
                from tail in GenerateIntegers(count - 1, sum - i)
                let prefix = Sequence.FromItems(i)
                select prefix.Concatenate(tail);
        }

        private static IEnumerable<ISequence<int>> GenerateSpacings(int length, int constraintCount, int constraintSum)
        {
            Debug.Assert(length >= 0);
            Debug.Assert(constraintCount >= 0);
            Debug.Assert(constraintSum >= 0);

            var spacingCount = constraintCount + 1;
            var spacingSum = length - constraintSum - Math.Max(0, constraintCount - 1);

            if (spacingSum < 0)
            {
                return Enumerable.Empty<ISequence<int>>();
            }
            var numbers = GenerateIntegers(spacingCount, spacingSum);
            var deltas = Sequence.FromFunction(spacingCount, i => i == 0 || i == spacingCount - 1 ? 0 : 1);

            return numbers.Select(ns => ns.ZipWith(deltas, (x, y) => x + y));
        }

        private static IEnumerable<ISequence<Square>> GeneratePatterns(int totalSize, ISequence<int> constraints)
        {
            if (totalSize < 0)
            {
                throw new ArgumentOutOfRangeException("totalSize");
            }
            if (constraints == null)
            {
                throw new ArgumentNullException("constraints");
            }
            var constraintCount = constraints.Length;
            var constraintSum = constraints.Items.Sum();

            var spacings = GenerateSpacings(totalSize, constraintCount, constraintSum);

            var blocks = constraints.Map(n => Sequence.Repeat(n, Square.FILLED));

            foreach (var spacing in spacings)
            {
                var spaces = spacing.Map(n => Sequence.Repeat(n, Square.EMPTY));

                yield return spaces.Intersperse(blocks).Flatten();
            }
        }

        internal int SatisfiedPrefixLength(Slice slice)
        {
            var knownPrefix = slice.KnownPrefix;
            var knownConstraints = knownPrefix.DeriveConstraints();

            return Values.CommonPrefixLength(knownConstraints.Values);
        }

        internal Constraints Reverse()
        {
            return new Constraints(Values.Reverse());
        }

        internal int SatisfiedSuffixLength(Slice slice)
        {
            return Reverse().SatisfiedPrefixLength(slice.Reverse());
        }

        internal bool IsSatisfied(Slice slice)
        {
            if (slice.IsFullyKnown)
            {
                var derivedConstraints = slice.DeriveConstraints();

                return derivedConstraints.Equals(this);
            }
            return false;
        }

        internal Range UnsatisfiedValueRange(Slice slice)
        {
            var knownPrefix = slice.KnownPrefix;
            var rest = slice.Lift(xs => xs.DropPrefix(knownPrefix.Squares.Length));
            var knownSuffix = rest.KnownSuffix;

            var left = SatisfiedPrefixLength(knownPrefix);
            var right = Values.Length - Lift(x => x.DropPrefix(left)).SatisfiedSuffixLength(knownSuffix);

            return Range.FromStartAndEndExclusive(left, right);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Constraints);
        }

        public bool Equals(Constraints that)
        {
            return that != null && Values.Equals(that.Values);
        }

        public override int GetHashCode()
        {
            return Values.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("[{0}]", Values.Map(x => x.ToString()).Join("-"));
        }

        internal Constraints Lift(Func<ISequence<int>, ISequence<int>> function)
        {
            return new Constraints(function(Values));
        }

        internal void Generate(Action<bool[]> receiver, ISequence<Square> compatibleWith)
        {
            new Generator(receiver, compatibleWith, Values).Generate();
        }

        internal ISequence<Square> Superposition(ISequence<Square> compatibleWith)
        {
            Square[] result = null;

            Generate((bool[] bs) =>
            {
                if (result == null)
                {
                    result = bs.Select(b => b ? Square.FILLED : Square.EMPTY).ToArray();
                }
                else
                {
                    for (var i = 0; i != result.Length; ++i)
                    {
                        var sqr = bs[i] ? Square.FILLED : Square.EMPTY;

                        result[i] = result[i].Merge(sqr);
                    }
                }
            }, compatibleWith);

            return Sequence.FromItems(result);
        }

        private class Generator
        {
            private readonly Action<bool[]> receiver;

            private readonly bool[] slice;

            private readonly ISequence<Square> compatibleWith;

            private readonly ISequence<int> constraints;

            private readonly int[] cumulative;

            public Generator(Action<bool[]> receiver, ISequence<Square> compatibleWith, ISequence<int> constraints)
            {
                this.receiver = receiver;
                slice = new bool[compatibleWith.Length];
                this.compatibleWith = compatibleWith;
                this.constraints = constraints;
                cumulative = ComputeCumulative(constraints);
            }

            private static int[] ComputeCumulative(ISequence<int> constraints)
            {
                var result = new int[constraints.Length];

                if (result.Length > 0)
                {
                    result[result.Length - 1] = constraints[constraints.Length - 1];

                    for (var i = result.Length - 2; i >= 0; --i)
                    {
                        result[i] = constraints[i] + 1 + result[i + 1];
                    }
                }

                return result;
            }

            public void Generate()
            {
                Generate(0, 0);
            }

            private void Yield()
            {
                receiver(slice);
            }

            private void Generate(int constraintIndex, int index)
            {
                if (constraintIndex == constraints.Length)
                {
                    for (var i = index; i != slice.Length; ++i)
                    {
                        if (compatibleWith[i] == Square.FILLED)
                        {
                            return;
                        }

                        slice[i] = false;
                    }

                    Yield();
                }
                else if (slice.Length - index < cumulative[constraintIndex])
                {
                }
                else
                {
                    if (compatibleWith[index] != Square.FILLED)
                    {
                        slice[index] = false;

                        Generate(constraintIndex, index + 1);
                    }

                    var size = constraints[constraintIndex];
                    for (var i = 0; i != size; ++i)
                    {
                        if (compatibleWith[index] == Square.EMPTY)
                        {
                            return;
                        }

                        slice[index] = true;

                        ++index;
                    }

                    ++constraintIndex;

                    if (constraintIndex < constraints.Length)
                    {
                        if (compatibleWith[index] == Square.FILLED)
                        {
                            return;
                        }

                        slice[index] = false;

                        index++;
                    }

                    Generate(constraintIndex, index);
                }
            }
        }
    }
}