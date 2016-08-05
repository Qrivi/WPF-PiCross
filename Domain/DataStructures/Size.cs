using System;

namespace DataStructures
{
    public class Size
    {
        public Size(int width, int height)
        {
            if (width < 0)
            {
                throw new ArgumentOutOfRangeException("width");
            }
            if (height < 0)
            {
                throw new ArgumentOutOfRangeException("height");
            }
            Width = width;
            Height = height;
        }

        public int Width { get; }

        public int Height { get; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Size);
        }

        public bool Equals(Size that)
        {
            return that != null && Width == that.Width && Height == that.Height;
        }

        public override int GetHashCode()
        {
            return Width.GetHashCode() ^ Height.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("PuzzleSize[{0}, {1}]", Width, Height);
        }

        public static bool operator ==(Size s1, Size s2)
        {
            if (ReferenceEquals(s1, null))
            {
                return ReferenceEquals(s2, null);
            }
            return s1.Equals(s2);
        }

        public static bool operator !=(Size s1, Size s2)
        {
            return !(s1 == s2);
        }
    }
}