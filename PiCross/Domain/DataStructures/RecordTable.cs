using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStructures
{
    public interface IRecordTable<K, V>
    {
        bool HasRecordFor( K key );

        V GetRecordFor( K key );

        void RegisterValue( K key, V value );
    }

    public static class RecordTable
    {
        public static IRecordTable<K, V> FromComparer<K, V>( IComparer<V> comparer )
        {
            return new Table<K, V>( comparer );
        }

        public static IRecordTable<K, V> FromDefaultComparer<K, V>()
            where V : IComparable<V>
        {
            return FromComparer<K, V>( Comparer<V>.Default );
        }

        private class Table<K, V> : IRecordTable<K, V>
        {
            private readonly Dictionary<K, V> table;

            private readonly IComparer<V> comparer;

            public Table( IComparer<V> comparer )
            {
                if ( comparer == null )
                {
                    throw new ArgumentNullException( "comparer" );
                }
                else
                {
                    this.comparer = comparer;
                    table = new Dictionary<K, V>();
                }
            }

            public bool HasRecordFor( K key )
            {
                return table.ContainsKey( key );
            }

            public V GetRecordFor( K key )
            {
                return table[key];
            }

            public void RegisterValue( K key, V value )
            {
                if ( BeatsCurrentRecord( key, value ) )
                {
                    table[key] = value;
                }
            }

            private bool BeatsCurrentRecord( K key, V value )
            {
                return !HasRecordFor( key ) || comparer.Compare( GetRecordFor( key ), value ) < 0;
            }
        }
    }
}
