namespace Cells
{
    public class Util
    {
        public static bool AreEqual<T>(T x, T y)
        {
            if (x == null)
            {
                return y == null;
            }
            return x.Equals(y);
        }
    }
}