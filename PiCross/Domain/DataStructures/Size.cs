namespace PiCross.DataStructures
{
    public class Size
    {
        private readonly int width;

        private readonly int height;

        public Size( int width, int height )
        {
            this.width = width;
            this.height = height;
        }

        public int Width { get { return width; } }

        public int Height { get { return height; } }

        public override bool Equals( object obj )
        {
            return Equals( obj as Size );
        }

        public bool Equals( Size that )
        {
            return that != null && this.width == that.width && this.height == that.height;
        }

        public override int GetHashCode()
        {
            return width.GetHashCode() ^ height.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format( "PuzzleSize[{0}, {1}]", width, height );
        }
    }
}
