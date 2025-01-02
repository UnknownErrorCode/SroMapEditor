using System.Security.AccessControl;

namespace SimpleGridFly.JMXFiles.BSR
{
    public struct CObjectInfo
    {
        #region Fields

        public uint NameLength;
        private string name;
        private ResourceType type;
        private uint unkUInt0;
        private uint unkUInt1;

        #endregion Fields

        #region Properties

        public string Name { get => name; set => name = value; }
        public ResourceType Type { get => type; set => type = value; }
        public uint UnkUInt0 { get => unkUInt0; set => unkUInt0 = value; }
        public uint UnkUInt1 { get => unkUInt1; set => unkUInt1 = value; }

        #endregion Properties
    }
}