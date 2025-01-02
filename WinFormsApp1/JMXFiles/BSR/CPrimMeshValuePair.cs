namespace SimpleGridFly.JMXFiles.BSR
{
    public struct CPrimMeshValuePair
    {
        #region Properties

        private string meshPath;

        /// <summary>
        /// If (header.Int0 & 1)    =>   this->dword310 = 1
        /// </summary>
        private uint meshUnkUInt0;

        /// <summary>
        /// Path of mesh file.
        /// </summary>
        public string MeshPath { get => meshPath; set => meshPath = value; }

        public uint MeshUnkUInt0 { get => meshUnkUInt0; set => meshUnkUInt0 = value; }

        #endregion Properties
    }
}