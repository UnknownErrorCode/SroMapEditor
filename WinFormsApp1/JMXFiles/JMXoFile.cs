using WinFormsApp1;

namespace SimpleGridFly.JMXFiles
{
    internal class JMXoFile
    {
        /// <summary>
        /// JMX Header file of version
        /// </summary>
        private readonly JMXHeader header;

        /// <summary>
        /// X coordinate of .m file.
        /// </summary>
        private byte x;

        /// <summary>
        /// Y coordinate of .m file.
        /// </summary>
        private byte y;

        public Dictionary<int, OFileMapObject> Data { get; private set; } = new();

        public JMXoFile(byte[] buffer, byte xCoordinate = 0, byte yCoordinate = 0)
        {
            try
            {
                x = xCoordinate;
                y = yCoordinate;

                using (MemoryStream strea = new MemoryStream(buffer))
                {
                    using (BinaryReader reader = new BinaryReader(strea))
                    {
                        header = new JMXHeader(reader.ReadChars(12), JmxFileFormat._m);

                        for (byte zBlock = 0; zBlock < 6; zBlock++)
                        {
                            for (byte xBlock = 0; xBlock < 6; xBlock++)
                            {
                                var objCount = reader.ReadUInt16();
                                for (int i = 0; i < objCount; i++)
                                {
                                    OFileMapObject file = new OFileMapObject(reader);
                                    Data.Add(file.UniqueId, file);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                Initialized = false;
            }

            Initialized = true;
        }

        public bool Initialized { get; private set; }
    }
}