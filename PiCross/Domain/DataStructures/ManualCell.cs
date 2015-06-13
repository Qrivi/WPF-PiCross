using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiCross.DataStructures
{
    internal abstract class ManualCell<T> : Var<T>, ICell<T>
    {
        protected ManualCell( T initialValue )
            : base( initialValue )
        {
            // NOP
        }

        public override T Value
        {
            get
            {
                return ReadValue();
            }
            set
            {
                WriteValue( value );
            }
        }

        public bool IsDirty
        {
            get
            {
                return !AreEqual( ReadValue(), base.Value );
            }
        }

        public void Refresh()
        {
            if ( IsDirty )
            {
                base.Value = ReadValue();

                if ( PropertyChanged != null )
                {
                    PropertyChanged( this, new System.ComponentModel.PropertyChangedEventArgs( "Value" ) );
                }
            }
        }

        protected abstract T ReadValue();

        protected abstract void WriteValue( T value );

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    }

    internal class ReadonlyManualCell<T> : Var<T>, ICell<T>
    {
        private readonly Func<T> function;

        public ReadonlyManualCell( Func<T> function )
            : base( function() )
        {
            this.function = function;
        }

        public override T Value
        {
            get
            {
                return function();
            }
            set
            {
                throw new InvalidOperationException();
            }
        }

        public bool IsDirty
        {
            get
            {
                return !AreEqual( function(), base.Value );
            }
        }

        public void Refresh()
        {
            if ( IsDirty )
            {
                base.Value = function();

                if ( PropertyChanged != null )
                {
                    PropertyChanged( this, new System.ComponentModel.PropertyChangedEventArgs( "Value" ) );
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    }

    internal class DirtyCellFactory<T, CELL>
        where CELL : ManualCell<T>
    {
        private readonly List<CELL> cells;

        private readonly Func<T, CELL> factory;

        public DirtyCellFactory( Func<T, CELL> factory )
        {
            if ( factory == null )
            {
                throw new ArgumentNullException( "factory" );
            }
            else
            {
                this.cells = new List<CELL>();
                this.factory = factory;
            }
        }

        public CELL CreateCell(T value)
        {
            var cell = factory( value );

            cells.Add( cell );

            return cell;
        }

        public void Clean()
        {
            foreach ( var cell in cells )
            {
                cell.Refresh();
            }
        }
    }
}
