using OpenTK.Mathematics;

namespace SimpleGridFly.JMXFiles.BSR
{
    public struct CModDataParticleStruct
    {
        public uint IsEnabled;

        [Obsolete]
        public uint FileNameLength;

        /// <summary>
        /// .efp file
        /// </summary>
        public string FileName;

        [Obsolete]
        public uint BoneLength;

        public string Bone;

        /// <summary>
        /// relative to origin or bone if present
        /// </summary>
        public Vector3 BonePosition;

        /// <summary>
        /// in ms
        /// </summary>
        public uint BirthTime;

        public byte UnkByte0;
        public byte UnkByte1;
        public byte UnkByte2;
        public byte UnkByte3;

        /// <summary>
        /// if <see cref="UnkByte3"/> == 1
        /// <br>initial velocity?</br>
        /// </summary>
        public Vector3 UnkVector;
    }
}