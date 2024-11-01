using System;

namespace GridAsset
{
    [Serializable]
    public struct GridCoord : IEquatable<GridCoord>, IFormattable
    {
        public int x;
        public int y;

        public GridCoord(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public bool Equals(GridCoord other)
        {
            return x == other.x && y == other.y;
        }
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return "(" + x + ", " + y + ")";
        }
        public string ToString(string format)
        {
            return "(" + x + ", " + y + ")";
        }

        public static GridCoord operator +(GridCoord a, GridCoord b)
        {
            return new GridCoord(a.x + b.x, a.y + b.y);
        }
        public static GridCoord operator -(GridCoord a, GridCoord b)
        {
            return new GridCoord(a.x - b.x, a.y - b.y);
        }
        public static bool operator ==(GridCoord a, GridCoord b)
        {
            return a.x == b.x && a.y == b.y;
        }
        public static bool operator !=(GridCoord a, GridCoord b)
        {
            return a.x != b.x || a.y != b.y;
        }

        public override bool Equals(object other)
        {
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x, y);
        }
    }
}