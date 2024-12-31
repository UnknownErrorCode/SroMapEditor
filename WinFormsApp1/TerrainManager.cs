// TerrainManager.cs
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Structs;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using WinFormsApp1;

namespace SimpleGridFly
{
    public class TerrainManager
    {
        // Struct to hold terrain mesh data and its position
        public struct TerrainMesh
        {
            public int Vao;
            public int Vbo;
            public int VertexCount;
            public Vector3 Position; // World position based on X and Z
        }

        private readonly int _shaderProgram;
        private readonly int _uModelLoc;

        // Dictionary to store loaded terrains with their grid positions as keys
        public Dictionary<(int X, int Z), TerrainMesh> LoadedTerrains { get; private set; } = new Dictionary<(int X, int Z), TerrainMesh>();

        // List to store all available terrains with their grid positions
        private readonly List<(int X, int Z, string FilePath)> _allTerrains = new List<(int X, int Z, string FilePath)>();

        // Define the range around the camera to load terrains (e.g., 2 regions in each direction)
        private readonly int _loadRange = 3;

        // Region separation and block size
        public float RegionSeparation { get; private set; } = 1920f;

        private int _blocksPerRegion = 16;
        private float _blockSize = 120f;
        private float _cellScale = 7.5f; // Scale per cell

        public TerrainManager(int shaderProgram, int modelLocation)
        {
            _shaderProgram = shaderProgram;
            _uModelLoc = modelLocation;
            InitializeTextures("I:\\Clients\\Exay-Origin V1.014\\Map");
        }

        /// <summary>
        /// Indexes all available .m files in the specified root directory.
        /// Assumes directory structure: root\Z\X.m
        /// </summary>
        /// <param name="rootDirectory">Root directory containing Z subdirectories.</param>
        public void IndexTerrains(string rootDirectory)
        {
            if (!Directory.Exists(rootDirectory))
            {
                MessageBox.Show($"Directory does not exist: {rootDirectory}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Clear any existing entries
            _allTerrains.Clear();

            // Iterate through each subdirectory representing Z-coordinate
            var zDirectories = Directory.GetDirectories(rootDirectory);
            foreach (var zDir in zDirectories)
            {
                // Extract Z-coordinate from folder name
                string zDirName = Path.GetFileName(zDir);
                if (!int.TryParse(zDirName, out int zCoord))
                {
                    MessageBox.Show($"Invalid Z-coordinate folder name: {zDirName}", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    continue;
                }

                // Iterate through each .m file in the Z-coordinate folder
                var mFiles = Directory.GetFiles(zDir, "*.m");
                foreach (var mFile in mFiles)
                {
                    // Extract X-coordinate from filename
                    string xFileName = Path.GetFileNameWithoutExtension(mFile);
                    if (!int.TryParse(xFileName, out int xCoord))
                    {
                        MessageBox.Show($"Invalid X-coordinate filename: {xFileName}.m", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        continue;
                    }

                    // Add to the list of all terrains
                    _allTerrains.Add((xCoord, zCoord, mFile));
                }
            }

            Console.WriteLine($"Indexed {_allTerrains.Count} terrain regions.");
        }

        /// <summary>
        /// Updates loaded terrains based on the camera's current position.
        /// Loads terrains within the specified range and unloads those outside.
        /// </summary>
        /// <param name="cameraPosition">Current position of the camera.</param>
        public void UpdateLoadedTerrains(Vector3 cameraPosition)
        {
            // Determine the current region based on camera position
            int currentRegionX = (int)Math.Floor(cameraPosition.X / RegionSeparation);
            int currentRegionZ = (int)Math.Floor(cameraPosition.Z / RegionSeparation);

            // Define the range of regions to load
            int minX = currentRegionX - _loadRange;
            int maxX = currentRegionX + _loadRange;
            int minZ = currentRegionZ - _loadRange;
            int maxZ = currentRegionZ + _loadRange;

            // Create a set of regions that should be loaded
            HashSet<(int X, int Z)> regionsToLoad = new HashSet<(int X, int Z)>();
            for (int x = minX; x <= maxX; x++)
            {
                for (int z = minZ; z <= maxZ; z++)
                {
                    regionsToLoad.Add((x, z));
                }
            }

            // Load new terrains that are within the range and not yet loaded
            foreach (var region in regionsToLoad)
            {
                if (!LoadedTerrains.ContainsKey(region))
                {
                    // Find the corresponding file
                    var terrain = _allTerrains.Find(t => t.X == region.X && t.Z == region.Z);
                    if (terrain != default)
                    {
                        LoadTerrain(region.X, region.Z, terrain.FilePath);
                    }
                }
            }

            // Unload terrains that are no longer within the range
            var loadedRegions = new List<(int X, int Z)>(LoadedTerrains.Keys);
            foreach (var loadedRegion in loadedRegions)
            {
                if (!regionsToLoad.Contains(loadedRegion))
                {
                    UnloadTerrain(loadedRegion.X, loadedRegion.Z);
                }
            }
        }

        /// <summary>
        /// Calculates the world position of a region based on its grid coordinates.
        /// </summary>
        private Vector3 CalculateRegionPosition(int regionX, int regionZ)
        {
            return new Vector3(regionX * RegionSeparation, 0f, regionZ * RegionSeparation);
        }

        /// <summary>
        /// Loads a single terrain region.
        /// </summary>
        private void LoadTerrain(int regionX, int regionZ, string filePath)
        {
            try
            {
                byte[] fileData = File.ReadAllBytes(filePath);
                var region = new JMXmFile(fileData, xCoordinate: (byte)regionX, yCoordinate: (byte)regionZ);
                if (!region.Initialized)
                {
                    MessageBox.Show($"Failed to initialize terrain from file: {filePath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Generate mesh
                float[] terrainMesh2 = BuildRegionMesh(region);
                float[] terrainMesh = BuildRegionMeshWithTextures(region);
                int vertexCount = terrainMesh.Length / 9; // 9 floats per vertex (3 position, 3 normal, 3 color)

                // Create VAO and VBO
                int vao = GL.GenVertexArray();
                int vbo = GL.GenBuffer();

                GL.BindVertexArray(vao);
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
                GL.BufferData(BufferTarget.ArrayBuffer, terrainMesh.Length * sizeof(float), terrainMesh, BufferUsageHint.StaticDraw);

                int stride = 9 * sizeof(float);

                // Position attribute (location=0)
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
                GL.EnableVertexAttribArray(0);

                // Normal attribute (location=1)
                GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));
                GL.EnableVertexAttribArray(1);

                // Color attribute (location=2)
                GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, stride, 6 * sizeof(float));
                GL.EnableVertexAttribArray(2);

                GL.BindVertexArray(0);

                // Store terrain mesh with its world position
                TerrainMesh tm = new TerrainMesh
                {
                    Vao = vao,
                    Vbo = vbo,
                    VertexCount = vertexCount,
                    Position = CalculateRegionPosition(regionX, regionZ) // Correct positioning
                };

                LoadedTerrains.Add((regionX, regionZ), tm);

                Console.WriteLine($"Loaded terrain at Region X:{regionX}, Z:{regionZ}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading terrain {filePath}:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Unloads a single terrain region.
        /// </summary>
        private void UnloadTerrain(int regionX, int regionZ)
        {
            if (LoadedTerrains.TryGetValue((regionX, regionZ), out TerrainMesh tm))
            {
                GL.DeleteBuffer(tm.Vbo);
                GL.DeleteVertexArray(tm.Vao);
                LoadedTerrains.Remove((regionX, regionZ));

                Console.WriteLine($"Unloaded terrain at Region X:{regionX}, Z:{regionZ}");
            }
        }

        private float[] BuildRegionMesh(JMXmFile region)
        {
            List<float> vertices = new List<float>();

            const int BLOCKS_PER_ROW = 6; // Number of blocks per row in a 6x6 layout
            const int BLOCK_SIZE = 17;   // 16 cells per block, with an extra for edges
            const int CELL_SIZE = 20;    // Each cell spans 20 units
            const int BLOCK_STRIDE = (BLOCK_SIZE - 1) * CELL_SIZE; // Size of one block in world units

            for (int blockIndex = 0; blockIndex < 36; blockIndex++) // 6x6 grid of blocks
            {
                // Determine the block's X and Z position in the grid
                int blockX = blockIndex % BLOCKS_PER_ROW;
                int blockZ = blockIndex / BLOCKS_PER_ROW;

                if (!region.Blocks.TryGetValue(Point8.FromXY((byte)blockX, (byte)blockZ), out var mapBlock))
                    continue; // Skip missing blocks

                // Base global position of the block
                float baseX = blockX * BLOCK_STRIDE;
                float baseZ = blockZ * BLOCK_STRIDE;

                for (int cellX = 0; cellX < BLOCK_SIZE - 1; cellX++) // Iterate over cells in X direction
                {
                    for (int cellZ = 0; cellZ < BLOCK_SIZE - 1; cellZ++) // Iterate over cells in Z direction
                    {
                        // Heights from the block's MapCells
                        var cell0 = Point8.FromXY((byte)cellZ, (byte)cellX); // Swap X and Z
                        var cell1 = Point8.FromXY((byte)cellZ, (byte)(cellX + 1));
                        var cell2 = Point8.FromXY((byte)(cellZ + 1), (byte)cellX);
                        var cell3 = Point8.FromXY((byte)(cellZ + 1), (byte)(cellX + 1));

                        if (!mapBlock.MapCells.TryGetValue(cell0, out var meshCell0) ||
                            !mapBlock.MapCells.TryGetValue(cell1, out var meshCell1) ||
                            !mapBlock.MapCells.TryGetValue(cell2, out var meshCell2) ||
                            !mapBlock.MapCells.TryGetValue(cell3, out var meshCell3))
                        {
                            continue; // Skip missing cells
                        }

                        // Extract heights
                        float h0 = meshCell0.Height;
                        float h1 = meshCell1.Height;
                        float h2 = meshCell2.Height;
                        float h3 = meshCell3.Height;

                        // Compute global positions for the four vertices
                        Vector3 p0 = new Vector3(baseX + cellX * CELL_SIZE, h0, baseZ + cellZ * CELL_SIZE);
                        Vector3 p1 = new Vector3(baseX + (cellX + 1) * CELL_SIZE, h1, baseZ + cellZ * CELL_SIZE);
                        Vector3 p2 = new Vector3(baseX + cellX * CELL_SIZE, h2, baseZ + (cellZ + 1) * CELL_SIZE);
                        Vector3 p3 = new Vector3(baseX + (cellX + 1) * CELL_SIZE, h3, baseZ + (cellZ + 1) * CELL_SIZE);

                        // Compute normals for each triangle
                        Vector3 n0 = ComputeNormal(p0, p1, p2);
                        Vector3 n1 = ComputeNormal(p2, p1, p3);

                        // Optional: color based on average height
                        float avgH1 = (h0 + h1 + h2) / 3f;
                        float colorFactor1 = MathF.Min(avgH1 / 50f, 1f); // Adjust scaling as needed
                        Vector3 color1 = new Vector3(0f, colorFactor1, 0f); // Greenish

                        float avgH2 = (h1 + h2 + h3) / 3f;
                        float colorFactor2 = MathF.Min(avgH2 / 50f, 1f);
                        Vector3 color2 = new Vector3(0f, colorFactor2, 0f); // Greenish

                        // Add vertices for the two triangles
                        // Triangle 1: (p0, p1, p2)
                        AddVertex(vertices, p0, n0, color1);
                        AddVertex(vertices, p1, n0, color1);
                        AddVertex(vertices, p2, n0, color1);

                        // Triangle 2: (p2, p1, p3)
                        AddVertex(vertices, p2, n1, color2);
                        AddVertex(vertices, p1, n1, color2);
                        AddVertex(vertices, p3, n1, color2);
                    }
                }
            }

            return vertices.ToArray();
        }

        private float[] BuildRegionMeshWithTextures(JMXmFile region)
        {
            List<float> vertices = new List<float>();

            const int BLOCKS_PER_ROW = 6;
            const int BLOCK_SIZE = 17;
            const int CELL_SIZE = 20;
            const int BLOCK_STRIDE = (BLOCK_SIZE - 1) * CELL_SIZE;

            for (int blockIndex = 0; blockIndex < 36; blockIndex++)
            {
                int blockX = blockIndex % BLOCKS_PER_ROW;
                int blockZ = blockIndex / BLOCKS_PER_ROW;

                if (!region.Blocks.TryGetValue(Point8.FromXY((byte)blockX, (byte)blockZ), out var mapBlock))
                    continue;

                float baseX = blockX * BLOCK_STRIDE;
                float baseZ = blockZ * BLOCK_STRIDE;

                for (int cellX = 0; cellX < BLOCK_SIZE - 1; cellX++)
                {
                    for (int cellZ = 0; cellZ < BLOCK_SIZE - 1; cellZ++)
                    {
                        var cell0 = Point8.FromXY((byte)cellZ, (byte)cellX);
                        var cell1 = Point8.FromXY((byte)cellZ, (byte)(cellX + 1));
                        var cell2 = Point8.FromXY((byte)(cellZ + 1), (byte)cellX);
                        var cell3 = Point8.FromXY((byte)(cellZ + 1), (byte)(cellX + 1));

                        if (!mapBlock.MapCells.TryGetValue(cell0, out var meshCell0) ||
                            !mapBlock.MapCells.TryGetValue(cell1, out var meshCell1) ||
                            !mapBlock.MapCells.TryGetValue(cell2, out var meshCell2) ||
                            !mapBlock.MapCells.TryGetValue(cell3, out var meshCell3))
                        {
                            continue;
                        }

                        // Extract texture index
                        int textureIndex = meshCell0.Texture & 0x03FF; // Extract lower 10 bits
                        if (!TextureMap.TryGetValue(textureIndex, out int textureId))
                        {
                            Console.WriteLine($"Texture index {textureIndex} not found in texture map.");
                            continue;
                        }

                        float h0 = meshCell0.Height;
                        float h1 = meshCell1.Height;
                        float h2 = meshCell2.Height;
                        float h3 = meshCell3.Height;

                        Vector3 p0 = new Vector3(baseX + cellX * CELL_SIZE, h0, baseZ + cellZ * CELL_SIZE);
                        Vector3 p1 = new Vector3(baseX + (cellX + 1) * CELL_SIZE, h1, baseZ + cellZ * CELL_SIZE);
                        Vector3 p2 = new Vector3(baseX + cellX * CELL_SIZE, h2, baseZ + (cellZ + 1) * CELL_SIZE);
                        Vector3 p3 = new Vector3(baseX + (cellX + 1) * CELL_SIZE, h3, baseZ + (cellZ + 1) * CELL_SIZE);

                        Vector3 n0 = ComputeNormal(p0, p1, p2);
                        Vector3 n1 = ComputeNormal(p2, p1, p3);

                        Vector2 uv0 = new Vector2(0.0f, 0.0f);
                        Vector2 uv1 = new Vector2(1.0f, 0.0f);
                        Vector2 uv2 = new Vector2(0.0f, 1.0f);
                        Vector2 uv3 = new Vector2(1.0f, 1.0f);

                        // Add vertices for two triangles
                        AddVertexWithTexture(vertices, p0, n0, uv0, textureId);
                        AddVertexWithTexture(vertices, p1, n0, uv1, textureId);
                        AddVertexWithTexture(vertices, p2, n0, uv2, textureId);

                        AddVertexWithTexture(vertices, p2, n1, uv2, textureId);
                        AddVertexWithTexture(vertices, p1, n1, uv1, textureId);
                        AddVertexWithTexture(vertices, p3, n1, uv3, textureId);
                    }
                }
            }

            return vertices.ToArray();
        }

        private void AddVertexWithTexture(List<float> vertices, Vector3 pos, Vector3 normal, Vector2 texCoord, int textureId)
        {
            vertices.Add(pos.X);
            vertices.Add(pos.Y);
            vertices.Add(pos.Z);
            vertices.Add(normal.X);
            vertices.Add(normal.Y);
            vertices.Add(normal.Z);
            vertices.Add(texCoord.X);
            vertices.Add(texCoord.Y);
            vertices.Add(textureId);
        }

        /// <summary>
        /// Computes the normal vector for a triangle defined by three points.
        /// </summary>
        private Vector3 ComputeNormal(Vector3 p0, Vector3 p1, Vector3 p2)
        {
            Vector3 u = p1 - p0;
            Vector3 v = p2 - p0;
            Vector3 n = Vector3.Cross(u, v);
            n.Normalize();
            return n;
        }

        /// <summary>
        /// Adds a vertex's data to the list.
        /// </summary>
        private void AddVertex(List<float> list, Vector3 pos, Vector3 normal, Vector3 color)
        {
            // Position
            list.Add(pos.X);
            list.Add(pos.Y);
            list.Add(pos.Z);

            // Normal
            list.Add(normal.X);
            list.Add(normal.Y);
            list.Add(normal.Z);

            // Color
            list.Add(color.X);
            list.Add(color.Y);
            list.Add(color.Z);
        }

        /// <summary>
        /// Renders all loaded terrain regions.
        /// </summary>
        public void RenderTerrains()
        {
            foreach (var terrain in LoadedTerrains.Values)
            {
                Matrix4 model = Matrix4.CreateTranslation(terrain.Position);
                GL.UniformMatrix4(_uModelLoc, false, ref model);

                GL.BindVertexArray(terrain.Vao);
                GL.DrawArrays(PrimitiveType.Triangles, 0, terrain.VertexCount);
            }
        }

        /// <summary>
        /// Cleans up all loaded terrains by deleting their VAOs and VBOs.
        /// </summary>
        public void Cleanup()
        {
            foreach (var terrain in LoadedTerrains.Values)
            {
                GL.DeleteBuffer(terrain.Vbo);
                GL.DeleteVertexArray(terrain.Vao);
            }
            LoadedTerrains.Clear();
        }

        private string[] TexPaths; // Array to store texture file paths
        private Dictionary<int, int> TextureMap = new(); // Maps texture indices to OpenGL texture IDs

        /// <summary>
        /// Loads texture file paths from tile2d.ifo and initializes OpenGL textures.
        /// </summary>
        public void InitializeTextures(string mapFolderPath)
        {
            string ifoPath = $"{mapFolderPath}\\tile2d.ifo";

            if (!File.Exists(ifoPath))
            {
                MessageBox.Show("Could not find tile2d.ifo. Terminating application.");
                Environment.Exit(1);
            }

            List<TextureInfo> textureInfos = ParseTextureFile(ifoPath);

            foreach (var texture in textureInfos)
            {
                string texturePath = $"{mapFolderPath}\\tile2d\\{texture.TexturePath}".Replace(".ddj", ".png");

                int textureId = LoadTexture(texturePath);
                if (textureId >= 0)
                {
                    TextureMap[texture.Id] = textureId; // Map texture index to OpenGL texture ID
                }
                else
                {
                    Console.WriteLine($"Failed to load texture: {texturePath}");
                }
            }

            MessageBox.Show($"Loaded {TextureMap.Count} textures.");
        }

        public List<TextureInfo> ParseTextureFile(string filePath)
        {
            var textureList = new List<TextureInfo>();

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File not found: {filePath}");
                return textureList;
            }

            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                // Skip empty or invalid lines
                if (string.IsNullOrWhiteSpace(line)) continue;

                // Match the pattern: ID, Secondary ID, "Region", "Texture Path"
                var match2 = Regex.Match(line, @"^(\d+)\s+(\S+)\s+""([^""]+)""\s+""([^""]+)""$");
                var match = Regex.Match(line, @"^(\d+)\s+(\S+)\s+""([^""]+)""\s+""([^""]+)""(?:\s+(.*))?$");

                if (match.Success)
                {
                    var textureInfo = new TextureInfo
                    {
                        Id = int.Parse(match.Groups[1].Value),
                        SecondaryId = match.Groups[2].Value,
                        Region = match.Groups[3].Value,
                        TexturePath = match.Groups[4].Value
                    };

                    textureList.Add(textureInfo);
                }
                else
                {
                    Console.WriteLine($"Failed to parse line: {line}");
                }
            }

            return textureList;
        }

        public class TextureInfo
        {
            public int Id { get; set; }           // First ID
            public string SecondaryId { get; set; } // Second ID (as a string)
            public string Region { get; set; }    // First quoted string
            public string TexturePath { get; set; } // Second quoted string
        }

        /// <summary>
        /// Loads a texture into OpenGL and returns the texture ID.
        /// </summary>
        /// <param name="texturePath">The file path of the texture.</param>
        /// <returns>The OpenGL texture ID, or -1 if the texture failed to load.</returns>
        private int LoadTexture(string texturePath)
        {
            if (!File.Exists(texturePath))
            {
                Console.WriteLine($"Texture file not found: {texturePath}");
                return -1;
            }

            int textureId = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, textureId);

            using Bitmap bitmap = new Bitmap(texturePath);
            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                                              ImageLockMode.ReadOnly,
                                              System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                          OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            bitmap.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            // GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.BindTexture(TextureTarget.Texture2D, 0);
            return textureId;
        }

        public void TestTextureRendering(int textureId)
        {
            // Use a simple quad with hardcoded UV coordinates
            float[] quadVertices = {
        // positions        // tex coords
        -1.0f, -1.0f, 0.0f, 0.0f, 0.0f, // Bottom-left
         1.0f, -1.0f, 0.0f, 1.0f, 0.0f, // Bottom-right
        -1.0f,  1.0f, 0.0f, 0.0f, 1.0f, // Top-left
         1.0f,  1.0f, 0.0f, 1.0f, 1.0f  // Top-right
    };

            int vao = GL.GenVertexArray();
            int vbo = GL.GenBuffer();

            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, quadVertices.Length * sizeof(float), quadVertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.BindTexture(TextureTarget.Texture2D, textureId);

            GL.UseProgram(_shaderProgram);
            GL.Uniform1(GL.GetUniformLocation(_shaderProgram, "uTexture"), 0);
            GL.BindVertexArray(vao);
            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);


            // Cleanup
            GL.BindVertexArray(0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }


    }
}