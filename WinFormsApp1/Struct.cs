namespace Structs
{
    public struct Point8
    {
        public byte X { get; set; }
        public byte Y { get; set; }

        public Point8(byte x, byte y)
        {
            X = x;
            Y = y;
        }

        public static Point8 FromXY(byte x, byte y)
        {
            return new Point8(x, y);
        }

        // Override Equals and GetHashCode for dictionary key usage
        public override bool Equals(object obj)
        {
            if (obj is Point8 other)
                return X == other.X && Y == other.Y;
            return false;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() * 397 ^ Y.GetHashCode();
        }
    }

    public struct Point32
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point32(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class CMapMeshCell
    {
        public float Height { get; set; }
        public ushort Texture { get; set; }
        public byte Brightness { get; set; }
    }

    public class CMapMeshTile
    {
        public byte ExtraMin { get; set; }
        public byte ExtraMax { get; set; }
    }

    public class CMapMeshBlock
    {
        public string BlockName { get; set; }
        public Dictionary<Point8, CMapMeshCell> MapCells { get; set; }
        public byte WaterType { get; set; }
        public byte WaterWaveType { get; set; }
        public float SeaLevel { get; set; }
        public CMapMeshTile[] MapMeshTiles { get; set; }
        public float HeightMax { get; set; }
        public float HeightMin { get; set; }
        public byte[] Reserved { get; set; }
    }
}