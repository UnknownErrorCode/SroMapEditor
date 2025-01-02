using Structs;

namespace WinFormsApp1
{
    public class JMXmFile
    {
        /// <summary>
        /// JMX Header file of version
        /// </summary>
        private readonly JMXHeader header;

        /// <summary>
        /// Each region consists of 36 Blocks. 6 x 6 Blocks equals 1 WorldRegion.
        /// <br>__________________ </br>
        /// <br>|1 |2 |3 |4 |5 |6 |</br>
        /// <br>|7 |8 |9 |10|11|12|</br>
        /// <br>|13|14|15|16|17|18|</br>
        /// <br>|19|20|21|22|23|24|</br>
        /// <br>|25|26|27|28|29|30|</br>
        /// <br>|31|32|33|34|35|36|</br>
        /// </summary>
        private readonly Dictionary<Point8, CMapMeshBlock> blocks = new Dictionary<Point8, CMapMeshBlock>(36);

        /// <summary>
        /// X coordinate of .m file.
        /// </summary>
        private byte x;

        /// <summary>
        /// Y coordinate of .m file.
        /// </summary>
        private byte y;

        /// <summary>
        /// .m file inside Map.Pk2 includes all informations about the terrain mesh.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="xCoordinate"></param>
        /// <param name="yCoordinate"></param>
        public JMXmFile(byte[] buffer, byte xCoordinate = 0, byte yCoordinate = 0)
        {
            try
            {
                X = xCoordinate;
                Z = yCoordinate;

                using (MemoryStream strea = new MemoryStream(buffer))
                {
                    using (BinaryReader reader = new BinaryReader(strea))
                    {
                        header = new JMXHeader(reader.ReadChars(12), JmxFileFormat._m);

                        for (byte zBlock = 0; zBlock < 6; zBlock++)
                        {
                            for (byte xBlock = 0; xBlock < 6; xBlock++)
                            {
                                Blocks.Add(Point8.FromXY(xBlock, zBlock), ReadBlock(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                Initialized = false;
            }
        }

        public bool Initialized { get; } = true;

        public Dictionary<Point8, CMapMeshBlock> Blocks => blocks;

        public byte X { get => x; set => x = value; }
        public byte Z { get => y; set => y = value; }

        public JMXHeader Header => header;

        /// <summary>
        /// Reads a single <see cref="CMapMeshBlock"/> from the <see cref="BinaryReader"/> <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private CMapMeshBlock ReadBlock(BinaryReader reader)
        {
            CMapMeshBlock block = new CMapMeshBlock()
            {
                BlockName = new string(System.Text.Encoding.UTF8.GetChars(reader.ReadBytes(6)))
            };

            #region Mesh Cells

            block.MapCells = new Dictionary<Point8, CMapMeshCell>(289);

            //every block has 17 * 17 MapMeshVerticies
            for (byte CellZ = 0; CellZ < 17; CellZ++)
            {
                for (byte CellX = 0; CellX < 17; CellX++)
                {
                    var meshCell = new CMapMeshCell()
                    {
                        Height = reader.ReadSingle(),
                        Texture = reader.ReadUInt16(),
                        Brightness = reader.ReadByte(),
                    };
                    block.MapCells.Add(new Point8(CellZ, CellX), meshCell);
                }
            }

            #endregion Mesh Cells

            #region Water

            block.WaterType = reader.ReadByte();
            block.WaterWaveType = reader.ReadByte();
            block.SeaLevel = reader.ReadSingle();

            #endregion Water

            #region Mesh Tiles

            block.MapMeshTiles = new CMapMeshTile[256];
            //TODO: Maybe also using dictionary later as property? idk...
            for (int mapMeshTile = 0; mapMeshTile < 256; mapMeshTile++)
            {
                block.MapMeshTiles[mapMeshTile] = new CMapMeshTile() { ExtraMin = reader.ReadByte(), ExtraMax = reader.ReadByte() };
            }

            #endregion Mesh Tiles

            #region Min Max Heigth

            block.HeightMax = reader.ReadSingle();
            block.HeightMin = reader.ReadSingle();
            block.Reserved = reader.ReadBytes(20);

            #endregion Min Max Heigth

            return block;
        }
    }

    /// <summary>
    /// Consists of various water and higth information.
    /// <br>17 * 17 <see cref="CMapMeshCell"/></br>
    /// <br>16 * 16 <see cref="CMapMeshTile"/></br>
    /// </summary>
    public struct CMapMeshBlock
    {
        /// <summary>
        /// Name of the Block
        /// </summary>
        public string BlockName { get; set; }

        /// <summary>
        /// unknown
        /// </summary>
        public CMapMeshTile[] MapMeshTiles { get; set; }

        /// <summary>
        /// Max height of block.
        /// </summary>
        public float HeightMax { get; set; }

        /// <summary>
        /// Min height of block.
        /// </summary>
        public float HeightMin { get; set; }

        /// <summary>
        /// Dictionary of all MapMeshCells. each Block consists of 16x16 MapmeshCells
        /// </summary>
        public Dictionary<Point8, CMapMeshCell> MapCells { get; set; }

        /// <summary>
        /// unknown reserved fixed 20 byte
        /// </summary>
        public byte[] Reserved { get; set; }

        /// <summary>
        /// Sea level height
        /// </summary>
        public float SeaLevel { get; set; }

        /// <summary>
        /// 0x00 = Water, 0x01 = Ice, FF = Solid
        /// </summary>
        public byte WaterType { get; set; }

        /// <summary>
        /// related to Block.Density (see screens)
        /// </summary>
        public byte WaterWaveType { get; set; }

        /// <summary>
        /// Returns the Cell of this block by x and y coordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>MapMeshCell cell</returns>
        public CMapMeshCell GetCellFromBlock(byte x, byte y)
        {
            return GetCellFromBlock(Point8.FromXY(x, y));
        }

        /// <summary>
        /// Returns the Cell of this block by System.Drawing.Point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns>MapMeshCell cell</returns>
        public CMapMeshCell GetCellFromBlock(Point8 point)
        {
            return MapCells.ContainsKey(point) ? MapCells[point] : new CMapMeshCell();
        }
    }

    public struct CMapMeshTile
    {
        public byte ExtraMin;
        public byte ExtraMax;
    }

    /// <summary>
    /// Has a <see cref="Height"/>,
    /// <br><see cref="Texture"/> 10 bit texture n 6 bit splash flag</br>
    /// <br><see cref="Brightness"/></br>
    /// </summary>
    public struct CMapMeshCell
    {
        /// <summary>
        /// Height of <see cref="CMapMeshCell"/>.
        /// </summary>
        public float Height;

        /// <summary>
        /// 10 bit texture n 6 bit splash
        /// </summary>
        public ushort Texture;

        /// <summary>
        /// Brightness of <see cref="CMapMeshCell"/>.
        /// </summary>
        public byte Brightness;
    }

    public struct JMXHeader : IPaddedString
    {
        public JMXHeader(char[] value, JmxFileFormat file)
        {
            Value = new string(value);
            File = file;
        }

        public int Length => 12;

        public string Value { get; set; }

        public JmxFileFormat File { get; }
    }

    public enum JmxFileFormat : byte
    {
        _m,
        _o,
        _o2,
        _t,
        _ifo,
        _2dt,
        _bsr,
        _bmt,
        _bms,
        _bsk,
        _ban,
        _efp,
        _ddj,
        _nav,
        _navDat,
        _dof,
        _wav,
        _snd,
    }

    public interface IPaddedString
    {
        int Length { get; }
        string Value { get; set; }
    }
}