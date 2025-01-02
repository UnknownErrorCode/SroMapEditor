using OpenTK.Mathematics;

namespace SimpleGridFly.JMXFiles
{
    public class OFileMapObject
    {
        private uint ObjID;             // from m_sObjectInfo (object.ifo)
        private Vector3 LocalPosition;  // relative to the region
        private short IsStatic;         // 0 = No, 0xFFFF = Yes
        private float Yaw;
        private short UID;              // Unique Id to track the same objects on multiple blocks
        private short Short0;
        private byte IsBig;             // exceeds region size
        private byte IsStruct;          // has "objectstring.ifo"

        public short UniqueId => UID;

        public OFileMapObject(BinaryReader reader)
        {
            // Read ObjID (uint)
            ObjID = reader.ReadUInt32();

            // Read LocalPosition (Vector3: 3 floats)
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            float z = reader.ReadSingle();
            LocalPosition = new Vector3(x, y, z);

            // Read IsStatic (short)
            IsStatic = reader.ReadInt16();

            // Read Yaw (float)
            Yaw = reader.ReadSingle();

            // Read UID (short)
            UID = reader.ReadInt16();

            // Read Short0 (short)
            Short0 = reader.ReadInt16();

            // Read IsBig (byte)
            IsBig = reader.ReadByte();

            // Read IsStruct (byte)
            IsStruct = reader.ReadByte();
        }

        // Optional: Add getters for the fields if needed
        public uint GetObjID() => ObjID;

        public Vector3 GetLocalPosition() => LocalPosition;

        public short GetIsStatic() => IsStatic;

        public float GetYaw() => Yaw;

        public short GetUID() => UID;

        public short GetShort0() => Short0;

        public byte GetIsBig() => IsBig;

        public byte GetIsStruct() => IsStruct;

        public override string ToString()
        {
            return $"ObjID: {ObjID}, LocalPosition: {LocalPosition}, IsStatic: {IsStatic}, Yaw: {Yaw}, UID: {UID}, Short0: {Short0}, IsBig: {IsBig}, IsStruct: {IsStruct}";
        }
    }
}