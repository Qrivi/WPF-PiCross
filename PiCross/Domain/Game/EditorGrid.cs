﻿using PiCross.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiCross.Game
{
    public class EditorGrid
    {
        private readonly IGrid<IVar<Square>> grid;

        private EditorGrid( IGrid<IVar<Square>> grid )
        {
            this.grid = grid;
        }

        public EditorGrid( IGrid<Square> grid )
            : this( grid.Map( x => new Var<Square>( x ) ).Copy() )
        {
            // NOP
        }

        public EditorGrid( int width, int height )
            : this( Grid.Create( width, height, _ => new Var<Square>( Square.UNKNOWN ) ) )
        {
            // NOP
        }

        public static EditorGrid FromStrings( params string[] rows )
        {
            var grid = Grid.CreateCharacterGrid( rows ).Map( Square.FromSymbol );

            return new EditorGrid( grid );
        }

        public IGrid<IVar<Square>> Contents
        {
            get
            {
                return grid;
            }
        }

        public IGrid<Square> Squares
        {
            get
            {
                return grid.Map( var => var.Value );
            }
        }

        public Slice Column( int x )
        {
            return new Slice( Squares.Column( x ) );
        }

        public Slice Row( int y )
        {
            return new Slice( Squares.Row( y ) );
        }

        public IEnumerable<Slice> Columns
        {
            get
            {
                return grid.ColumnIndices.Select( Column );
            }
        }

        public IEnumerable<Slice> Rows
        {
            get
            {
                return grid.RowIndices.Select( Row );
            }
        }

        public ISequence<Constraints> DeriveColumnConstraints()
        {
            return Sequence.FromEnumerable( Columns.Select( column => column.DeriveConstraints() ) );
        }

        public ISequence<Constraints> DeriveRowConstraints()
        {
            return Sequence.FromEnumerable( Rows.Select( row => row.DeriveConstraints() ) );
        }
    }
}
