namespace SimpleGridFly
{
    public class MapObjectManager
    {
        // List to store all map objects
        public List<MapObject> MapObjects { get; private set; } = new List<MapObject>();

        public MapObjectManager()
        {
        }

        public bool Initilalize(string rawDataPath)
        {
            string ifoPath = $"{rawDataPath}\\object.ifo";
            MapObjects.Clear();
            if (!File.Exists(ifoPath))
            {
                MessageBox.Show("Could not find object.ifo. Terminating application.");
                return false;
            }
            string[] rawData = File.ReadAllLines(ifoPath);

            ReadObjectInfo(rawData);
            return true;
        }

        private void ReadObjectInfo(string[] rawData)
        {
            foreach (var line in rawData)
            {
                var mapObject = ParseLine(line);
                if (mapObject != null)
                {
                    MapObjects.Add(mapObject.Value);
                }
            }
        }

        /// <summary>
        /// Parses a single line of raw data into a MapObject struct.
        /// </summary>
        /// <param name="line">A line of raw data.</param>
        /// <returns>A MapObject struct or null if parsing fails.</returns>
        private MapObject? ParseLine(string line)
        {
            var pattern = @"^(\d{5})\s+(0x[0-9a-fA-F]{8})\s+\""(.+?)\""$";

            var match = System.Text.RegularExpressions.Regex.Match(line, pattern);

            if (match.Success)
            {
                return new MapObject
                {
                    Id = int.Parse(match.Groups[1].Value),
                    Flags = match.Groups[2].Value,
                    FilePath = match.Groups[3].Value
                };
            }

            return null;
        }
    }

    public struct MapObject
    {
        public int Id { get; set; }
        public string Flags { get; set; }
        public string FilePath { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}, Flags: {Flags}, FilePath: {FilePath}";
        }
    }
}