namespace SimpleGridFly.JMXFiles.BSR
{
    public struct CModDataSet
    {
        #region Fields

        /// <summary>
        /// from PrimAnimationType
        /// </summary>
        public AnimationType AniType;

        [Obsolete]
        public List<IModData> ModDataTypes;

        public uint modSetDataCnt;
        public string Name;
        public uint NameLength;

        /// <summary>
        /// Locomotion = 0, Simple = 1, Ambient = 2,
        /// </summary>
        public uint Type;

        #endregion Fields
    }
}