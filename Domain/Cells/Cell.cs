﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Cells
{
    public abstract class Cell<T> : Var<T>, INotifyPropertyChanged
    {
        private PropertyChangedEventHandler PropertyChanged = (obj, args) => { };

        protected Cell(T initialValue = default(T))
            : base(initialValue)
        {
            // NOP
        }

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add
            {
                lock (PropertyChanged)
                {
                    PropertyChanged += value;
                }
            }
            remove
            {
                lock (PropertyChanged)
                {
                    PropertyChanged -= value;
                }
            }
        }

        public event Action ValueChanged
        {
            add { PropertyChanged += (obj, args) => value(); }
            remove { throw new NotSupportedException(); }
        }

        protected void NotifyObservers()
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Value"));
            }
        }

        public abstract void Refresh();

        public Cell<T> AsReadOnly()
        {
            return new ReadonlyWrapper<T>(this);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Cell<T>);
        }

        public bool Equals(Cell<T> that)
        {
            if (that == null)
            {
                return false;
            }
            if (Value == null)
            {
                return that.Value == null;
            }
            return Value.Equals(that.Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("CELL[{0}]", Value);
        }
    }

    public static class Cell
    {
        public static Cell<T> Create<T>(T initialValue = default(T))
        {
            return new ConcreteCell<T>(initialValue);
        }

        public static Cell<T> CreateFuture<T>()
        {
            return new FutureCell<T>();
        }

        private static void RegisterObserver<T, R>(Derived<R> derived, Cell<T> cell)
        {
            cell.ValueChanged += derived.Refresh;
        }

        public static Cell<R> Derived<R>(Func<R> function)
        {
            return new Derived<R>(function);
        }

        public static Cell<R> Derived<T, R>(Cell<T> cell, Func<T, R> function)
        {
            var derived = new Derived<R>(() => function(cell.Value));

            RegisterObserver(derived, cell);

            return derived;
        }

        public static Cell<R> Derived<T, R>(Cell<T> cell, Func<T, R> convert, Func<R, T> convertBack)
        {
            var derived = new Derived<R>(() => convert(cell.Value), x => { cell.Value = convertBack(x); });

            RegisterObserver(derived, cell);

            return derived;
        }

        public static Cell<R> Derived<T1, T2, R>(Cell<T1> c1, Cell<T2> c2, Func<T1, T2, R> function)
        {
            var derived = new Derived<R>(() => function(c1.Value, c2.Value));

            RegisterObserver(derived, c1);
            RegisterObserver(derived, c2);

            return derived;
        }

        public static Cell<R> Derived<T1, T2, T3, R>(Cell<T1> c1, Cell<T2> c2, Cell<T3> c3, Func<T1, T2, T3, R> function)
        {
            var derived = new Derived<R>(() => function(c1.Value, c2.Value, c3.Value));

            RegisterObserver(derived, c1);
            RegisterObserver(derived, c2);
            RegisterObserver(derived, c3);

            return derived;
        }

        public static Cell<R> Derived<T1, T2, T3, T4, R>(Cell<T1> c1, Cell<T2> c2, Cell<T3> c3, Cell<T4> c4,
            Func<T1, T2, T3, T4, R> function)
        {
            var derived = new Derived<R>(() => function(c1.Value, c2.Value, c3.Value, c4.Value));

            RegisterObserver(derived, c1);
            RegisterObserver(derived, c2);
            RegisterObserver(derived, c3);
            RegisterObserver(derived, c4);

            return derived;
        }

        public static Cell<R> Derived<T1, T2, T3, T4, T5, R>(Cell<T1> c1, Cell<T2> c2, Cell<T3> c3, Cell<T4> c4,
            Cell<T5> c5, Func<T1, T2, T3, T4, T5, R> function)
        {
            var derived = new Derived<R>(() => function(c1.Value, c2.Value, c3.Value, c4.Value, c5.Value));

            RegisterObserver(derived, c1);
            RegisterObserver(derived, c2);
            RegisterObserver(derived, c3);
            RegisterObserver(derived, c4);
            RegisterObserver(derived, c5);

            return derived;
        }

        public static Cell<R> Derived<T, R>(IEnumerable<Cell<T>> cells, Func<IEnumerable<T>, R> function)
        {
            var derived = new Derived<R>(() => function(cells.Select(cell => cell.Value)));

            foreach (var cell in cells)
            {
                RegisterObserver(derived, cell);
            }

            return derived;
        }
    }
}