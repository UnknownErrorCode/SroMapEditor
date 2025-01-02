namespace SimpleGridFly.JMXFiles.BSR
{
    public class CModDataMultiTextureRev : IModData
    {
        #region Constructors

        public CModDataMultiTextureRev(CModData data)
        {
            Data = data;
            Type = ModDataType.ModDataMultiTexRev;
        }

        #endregion Constructors

        #region Properties

        public CModData Data { get; }
        public ModDataType Type { get; }

        public uint unkUInt5 { get; set; }

        [Obsolete]
        public uint TextureLength { get; set; }

        /// <summary>
        /// .ddj
        /// </summary>
        public string Texture { get; set; }

        public uint unkUInt6 { get; set; }

        #endregion Properties
    }
}