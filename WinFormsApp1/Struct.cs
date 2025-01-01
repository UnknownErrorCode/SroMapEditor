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

    public struct TextureInfo
    {
        public int Id { get; set; }           // First ID
        public string SecondaryId { get; set; } // Second ID (as a string)
        public string Region { get; set; }    // First quoted string
        public string TexturePath { get; set; } // Second quoted string
    }
}