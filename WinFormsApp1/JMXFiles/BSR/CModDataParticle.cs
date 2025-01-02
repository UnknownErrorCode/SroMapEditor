namespace SimpleGridFly.JMXFiles.BSR
{
    public class CModDataParticle : IModData
    {
        public CModDataParticle(CModData data)
        {
            Data = data;
            Type = ModDataType.ModDataParticle;
        }

        #region Properties

        public ModDataType Type { get; }
        public CModData Data { get; }

        [Obsolete]
        public uint ParticleCount { get; set; }

        public CModDataParticleStruct[] Particles { get; set; }

        #endregion Properties
    }
}