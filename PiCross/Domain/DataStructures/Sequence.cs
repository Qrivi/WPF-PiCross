using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiCross.DataStructures
{
    public interface ISequence<out T> : IEnumerable<T>
    {
        int Length { get; }

        T this[int index] { get; }

        IEnumerable<int> Indices { get; }
    }

    public static class Sequence
    {
        public static ISequence<T> FromItems<T>(params T[] items)
        {
            return new ArraySequence<T>( items.Length, i => items[i] );
        }
    }

    internal abstract class SequenceBase<T> : ISequence<T>
    {
        public abstract int Length { get; }

        public abstract T this[int index] { get; }

        public IEnumerable<int> Indices
        {
            get
            {
                return Enumerable.Range( 0, this.Length );
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.Indices.Select( i => this[i] ).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    internal class ArraySequence<T> : SequenceBase<T>
    {
        private readonly T[] items;

        public ArraySequence(int length, Func<int, T> initializer)
        {
            items = Enumerable.Range(0, length).Select(initializer).ToArray();
        }

        public override int Length
        {
            get{
                return items.Length;
            }
        }

        public override T this[int index]
        {
            get
            {
                return items[index];
            }
        }
    }
}
