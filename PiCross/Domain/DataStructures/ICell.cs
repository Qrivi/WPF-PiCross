using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiCross.DataStructures
{
    public interface IVar<T>
    {
        T Value { get; set; }
    }

    public interface ICell<T> : IVar<T>, INotifyPropertyChanged
    {
        // Empty
    }

    public interface IDerived<T> : ICell<T>
    {
        void Refresh();
    }

    public static class Cell
    {
        public static ICell<T> Create<T>( T initialValue = default(T) )
        {
            return new Cell<T>( initialValue );
        }

        private static void RegisterObserver<T, R>( Derived<R> derived, ICell<T> cell )
        {
            cell.PropertyChanged += ( sender, args ) => derived.Refresh();
        }

        public static IDerived<R> Derived<R>( Func<R> function )
        {
            return new Derived<R>( function );
        }

        public static IDerived<R> Derived<T, R>( ICell<T> cell, Func<T, R> function )
        {
            var derived = new Derived<R>( () => function( cell.Value ) );

            RegisterObserver( derived, cell );

            return derived;
        }

        public static IDerived<R> Derived<T1, T2, R>( ICell<T1> c1, ICell<T2> c2, Func<T1, T2, R> function )
        {
            var derived = new Derived<R>( () => function( c1.Value, c2.Value ) );

            RegisterObserver( derived, c1 );
            RegisterObserver( derived, c2 );

            return derived;
        }

        public static IDerived<R> Derived<T1, T2, T3, R>( ICell<T1> c1, ICell<T2> c2, ICell<T3> c3, Func<T1, T2, T3, R> function )
        {
            var derived = new Derived<R>( () => function( c1.Value, c2.Value, c3.Value ) );

            RegisterObserver( derived, c1 );
            RegisterObserver( derived, c2 );
            RegisterObserver( derived, c3 );

            return derived;
        }

        public static IDerived<R> Derived<T1, T2, T3, T4, R>( ICell<T1> c1, ICell<T2> c2, ICell<T3> c3, ICell<T4> c4, Func<T1, T2, T3, T4, R> function )
        {
            var derived = new Derived<R>( () => function( c1.Value, c2.Value, c3.Value, c4.Value ) );

            RegisterObserver( derived, c1 );
            RegisterObserver( derived, c2 );
            RegisterObserver( derived, c3 );
            RegisterObserver( derived, c4 );

            return derived;
        }

        public static IDerived<R> Derived<T1, T2, T3, T4, T5, R>( ICell<T1> c1, ICell<T2> c2, ICell<T3> c3, ICell<T4> c4, ICell<T5> c5, Func<T1, T2, T3, T4, T5, R> function )
        {
            var derived = new Derived<R>( () => function( c1.Value, c2.Value, c3.Value, c4.Value, c5.Value ) );

            RegisterObserver( derived, c1 );
            RegisterObserver( derived, c2 );
            RegisterObserver( derived, c3 );
            RegisterObserver( derived, c4 );
            RegisterObserver( derived, c5 );

            return derived;
        }

        public static IDerived<R> Derived<T, R>( IEnumerable<ICell<T>> cells, Func<IEnumerable<T>, R> function )
        {
            var derived = new Derived<R>( () => function( cells.Select( cell => cell.Value ) ) );

            foreach ( var cell in cells )
            {
                RegisterObserver( derived, cell );
            }

            return derived;
        }
    }

    public static class ICellExtensions
    {
        public static ICell<T> AsReadOnly<T>( this ICell<T> cell )
        {
            return new ReadonlyWrapper<T>( cell );
        }
    }
}
