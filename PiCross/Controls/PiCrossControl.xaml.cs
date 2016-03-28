using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using DataStructures;
using UIGrid = System.Windows.Controls.Grid;

namespace PiCross.Controls
{
    public partial class PiCrossControl : UserControl
    {
        public PiCrossControl()
        {
            InitializeComponent();
        }
        
        #region Thumbnail

        public UIElement Thumbnail
        {
            get { return (UIElement) GetValue( ThumbnailProperty ); }
            set { SetValue( ThumbnailProperty, value ); }
        }

        public static readonly DependencyProperty ThumbnailProperty =
            DependencyProperty.Register( "Thumbnail", typeof( UIElement ), typeof( PiCrossControl ), new PropertyMetadata( null, (obj, args) => ((PiCrossControl) obj).OnThumbnailChanged(args) ) );

        private void OnThumbnailChanged(DependencyPropertyChangedEventArgs args)
        {
            if ( args.OldValue != null )
            {
                var oldThumbnail = (UIElement) args.OldValue;

                this.grid.Children.Remove( oldThumbnail );
            }

            if ( args.NewValue != null )
            {
                var newThumbnail = (UIElement) args.NewValue;

                System.Windows.Controls.Grid.SetColumn( newThumbnail, 0 );
                System.Windows.Controls.Grid.SetRow( newThumbnail, 0 );

                this.grid.Children.Add( newThumbnail );
            }            
        }

        #endregion

        #region SquareTemplate

        public DataTemplate SquareTemplate
        {
            get { return (DataTemplate) GetValue( SquareTemplateProperty ); }
            set { SetValue( SquareTemplateProperty, value ); }
        }

        public static readonly DependencyProperty SquareTemplateProperty =
            DependencyProperty.Register( "SquareTemplate", typeof( DataTemplate ), typeof( PiCrossControl ), new PropertyMetadata( null, ( obj, args ) => ( (PiCrossControl) obj ).OnSquareTemplateChanged( args ) ) );

        private void OnSquareTemplateChanged( DependencyPropertyChangedEventArgs args )
        {
            ClearChildren();
            CreateChildren();
        }

        #endregion

        #region ColumnConstraintsTemplate

        public DataTemplate ColumnConstraintsTemplate
        {
            get { return (DataTemplate) GetValue( ColumnConstraintsTemplateProperty ); }
            set { SetValue( ColumnConstraintsTemplateProperty, value ); }
        }

        public static readonly DependencyProperty ColumnConstraintsTemplateProperty =
            DependencyProperty.Register( "ColumnConstraintsTemplate", typeof( DataTemplate ), typeof( PiCrossControl ), new PropertyMetadata( null, ( obj, args ) => ( (PiCrossControl) obj ).OnColumnConstraintsTemplateChanged( args ) ) );

        private void OnColumnConstraintsTemplateChanged( DependencyPropertyChangedEventArgs args )
        {
            RecreateChildren();
        }

        #endregion

        #region RowConstraintsTemplate

        public DataTemplate RowConstraintsTemplate
        {
            get { return (DataTemplate) GetValue( RowConstraintsTemplateProperty ); }
            set { SetValue( RowConstraintsTemplateProperty, value ); }
        }

        public static readonly DependencyProperty RowConstraintsTemplateProperty =
            DependencyProperty.Register( "RowConstraintsTemplate", typeof( DataTemplate ), typeof( PiCrossControl ), new PropertyMetadata( null, ( obj, args ) => ( (PiCrossControl) obj ).OnRowConstraintsTemplateChanged( args ) ) );

        private void OnRowConstraintsTemplateChanged( DependencyPropertyChangedEventArgs args )
        {
            RecreateChildren();
        }

        #endregion

        #region PuzzleData

        public IPuzzleData PuzzleData
        {
            get { return (IPuzzleData) GetValue( PuzzleDataProperty ); }
            set { SetValue( PuzzleDataProperty, value ); }
        }

        public static readonly DependencyProperty PuzzleDataProperty =
            DependencyProperty.Register( "PuzzleData", typeof( IPuzzleData ), typeof( PiCrossControl ), new PropertyMetadata( null, ( obj, args ) => ( (PiCrossControl) obj ).OnDataChanged( args ) ) );

        private void OnDataChanged( DependencyPropertyChangedEventArgs args )
        {
            RecreateAll();
        }

        #endregion

        #region Children

        private void RecreateAll()
        {
            ClearAll();
            CreateAll();
        }

        private void RecreateChildren()
        {
            ClearChildren();
            CreateChildren();
        }

        private void ClearAll()
        {
            ClearChildren();
            ClearGridLayout();
        }

        private void ClearChildren()
        {
            this.grid.Children.Clear();
        }

        private void ClearGridLayout()
        {
            ClearColumnDefinitions();
            ClearRowDefinitions();
        }

        private void ClearColumnDefinitions()
        {
            this.grid.ColumnDefinitions.Clear();
        }

        private void ClearRowDefinitions()
        {
            this.grid.RowDefinitions.Clear();
        }

        private void CreateAll()
        {
            CreateGrid();
        }

        private void CreateGrid()
        {
            CreateColumnDefinitions();
            CreateRowDefinitions();
            CreateChildren();
        }

        private void RecreateColumnDefinitions()
        {
            ClearColumnDefinitions();
            CreateColumnConstraintControls();
        }

        private void RecreateRowDefinitions()
        {
            ClearRowDefinitions();
            CreateRowConstraintControls();
        }

        private void CreateColumnDefinitions()
        {
            Debug.Assert( this.grid.ColumnDefinitions.Count == 0 );

            if ( PuzzleData != null )
            {
                this.grid.ColumnDefinitions.Add( new ColumnDefinition() { Width = GridLength.Auto } );

                for ( var i = 0; i != PuzzleData.ColumnConstraints.Length; ++i )
                {
                    this.grid.ColumnDefinitions.Add( new ColumnDefinition() { Width = GridLength.Auto } );
                }
            }
        }

        private void CreateRowDefinitions()
        {
            Debug.Assert( this.grid.RowDefinitions.Count == 0 );

            if ( PuzzleData != null )
            {
                this.grid.RowDefinitions.Add( new RowDefinition() { Height = GridLength.Auto } );

                for ( var i = 0; i != PuzzleData.ColumnConstraints.Length; ++i )
                {
                    this.grid.RowDefinitions.Add( new RowDefinition() { Height = GridLength.Auto } );
                }
            }
        }

        private void CreateChildren()
        {
            Debug.Assert( this.grid.Children.Count == 0 );

            AddThumbnailChild();
            CreateSquareControls();
            CreateConstraintControls();
        }

        private void AddThumbnailChild()
        {
            if ( Thumbnail != null )
            {
                Debug.Assert( !this.grid.Children.Contains( Thumbnail ) );

                this.grid.Children.Add( this.Thumbnail );
            }
        }

        private void CreateSquareControls()
        {
            if ( PuzzleData != null && SquareTemplate != null )
            {
                foreach ( var position in PuzzleData.Grid.AllPositions )
                {
                    var gridCol = position.X + 1;
                    var gridRow = position.Y + 1;
                    var squareData = PuzzleData.Grid[position];
                    var squareControl = (FrameworkElement) SquareTemplate.LoadContent();

                    squareControl.DataContext = squareData;
                    UIGrid.SetColumn( squareControl, gridCol );
                    UIGrid.SetRow( squareControl, gridRow );

                    this.grid.Children.Add( squareControl );
                }
            }
        }

        private void CreateConstraintControls()
        {
            CreateColumnConstraintControls();
            CreateRowConstraintControls();
        }

        private void CreateColumnConstraintControls()
        {
            if ( PuzzleData != null && ColumnConstraintsTemplate != null )
            {
                foreach ( var index in PuzzleData.ColumnConstraints.Indices )
                {
                    var columnIndex = index + 1;
                    var columnConstraintData = PuzzleData.ColumnConstraints[index];
                    var constraintsControl = (FrameworkElement) ColumnConstraintsTemplate.LoadContent();

                    constraintsControl.DataContext = columnConstraintData;
                    UIGrid.SetRow( constraintsControl, 0 );
                    UIGrid.SetColumn( constraintsControl, columnIndex );

                    this.grid.Children.Add( constraintsControl );
                }
            }
        }

        private void CreateRowConstraintControls()
        {
            if ( PuzzleData != null && RowConstraintsTemplate != null )
            {
                foreach ( var index in PuzzleData.RowConstraints.Indices )
                {
                    var rowIndex = index + 1;
                    var rowConstraintData = PuzzleData.RowConstraints[index];
                    var constraintsControl = (FrameworkElement) RowConstraintsTemplate.LoadContent();

                    constraintsControl.DataContext = rowConstraintData;
                    UIGrid.SetRow( constraintsControl, rowIndex );
                    UIGrid.SetColumn( constraintsControl, 0 );

                    this.grid.Children.Add( constraintsControl );
                }
            }
        }

        #endregion
    }

    public interface IPuzzleData
    {
        IGrid<object> Grid { get; }

        ISequence<object> ColumnConstraints { get; }

        ISequence<object> RowConstraints { get; }
    }    
}
