﻿// TerrainManager.cs
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SimpleGridFly.JMXFiles;
using SimpleGridFly.Texture;
using WinFormsApp1;

namespace SimpleGridFly
{
    public class TerrainManager
    {
        private readonly int _uModelLoc;

        // Dictionary to store loaded terrains with their grid positions as keys
        public Dictionary<(int X, int Z), TerrainMeshBase> LoadedTerrains { get; private set; } = new Dictionary<(int X, int Z), TerrainMeshBase>();

        public Dictionary<(int X, int Z), OFileMapObject[]> LoadedTerrainObjectss { get; private set; } = new Dictionary<(int X, int Z), OFileMapObject[]>();

        // List to store all available terrains with their grid positions
        private readonly List<(int X, int Z, string FilePath)> _allTerrains = new List<(int X, int Z, string FilePath)>();

        // List to store all available terrain objects with their grid positions
        private readonly List<(int X, int Z, string FilePath)> _allTerrainObjects = new List<(int X, int Z, string FilePath)>();

        public List<(int X, int Z, string FilePath)> AllTerrains => _allTerrains;

        // Define the range around the camera to load terrains (e.g., 2 regions in each direction)
        private readonly int _loadRange = 5;

        // Region separation and block size
        public float RegionSeparation { get; private set; } = 1920f;

        public TerrainManager()
        {
            _uModelLoc = ShaderManager._terrainUModelLoc;
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
            _allTerrainObjects.Clear();
            LoadedTerrains.Clear();

            // Iterate through each subdirectory representing Z-coordinate
            var zDirectories = Directory.GetDirectories(rootDirectory);
            foreach (var zDir in zDirectories)
            {
                // Extract Z-coordinate from folder name
                string zDirName = Path.GetFileName(zDir);
                if (!int.TryParse(zDirName, out int zCoord))
                {
                    Console.WriteLine($"Invalid Z-coordinate folder name: {zDirName}", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                        Console.WriteLine($"Invalid X-coordinate filename: {xFileName}.m", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        continue;
                    }

                    // Add to the list of all terrains
                    _allTerrains.Add((xCoord, zCoord, mFile));
                }

                // Iterate through each .m file in the Z-coordinate folder
                var oFiles = Directory.GetFiles(zDir, "*.o");
                foreach (var oFile in oFiles)
                {
                    // Extract X-coordinate from filename
                    string xFileName = Path.GetFileNameWithoutExtension(oFile);
                    if (!int.TryParse(xFileName, out int xCoord))
                    {
                        Console.WriteLine($"Invalid X-coordinate filename: {xFileName}.o", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        continue;
                    }

                    // Add to the list of all terrains
                    _allTerrainObjects.Add((xCoord, zCoord, oFile));
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
                        LoadTerrainMesh(region.X, region.Z, terrain.FilePath);
                    }
                }
                if (!LoadedTerrainObjectss.ContainsKey(region))
                {
                    // Find the corresponding file
                    var terrain = _allTerrainObjects.Find(t => t.X == region.X && t.Z == region.Z);
                    if (terrain != default)
                    {
                        LoadTerrainObjects(region.X, region.Z, terrain.FilePath);
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

        private void LoadTerrainObjects(int regionX, int regionZ, string filePath)
        {
            if (File.Exists(filePath))
            {
                byte[] oFileRaw = File.ReadAllBytes(filePath);
                var oF = new JMXFiles.JMXoFile(oFileRaw, (byte)regionX, (byte)regionZ);

                var tempList = new List<OFileMapObject>();
                foreach (var mapObj in oF.Data)
                {
                    tempList.Add(mapObj.Value);
                }

                LoadedTerrainObjectss[(regionX, regionZ)] = tempList.ToArray();
            }
        }

        /// <summary>
        /// Loads a single terrain region.
        /// </summary>
        private void LoadTerrainMesh(int regionX, int regionZ, string filePath)
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
                TerrainMesh terrain = new TerrainMesh(region);

                terrain.Generate(out int vao, out int vbo);

                // Store terrain mesh with its world position
                TerrainMeshBase mBase = new TerrainMeshBase()
                {
                    Vao = vao,
                    Vbo = vbo,
                    VertexCount = terrain.Vertices.Length / 9,
                    Position = CalculateRegionPosition(regionX, regionZ) // Correct positioning
                };
                LoadedTerrains.Add((regionX, regionZ), mBase);

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
            if (LoadedTerrains.TryGetValue((regionX, regionZ), out TerrainMeshBase tm))
            {
                GL.DeleteBuffer(tm.Vbo);
                GL.DeleteVertexArray(tm.Vao);
                LoadedTerrains.Remove((regionX, regionZ));

                Console.WriteLine($"Unloaded terrain at Region X:{regionX}, Z:{regionZ}");
            }
        }

        /// <summary>
        /// Renders all loaded terrain regions.
        /// </summary>
        public void RenderTerrains(Matrix4 view, Matrix4 proj)
        {
            // Render terrains using the terrain shader
            GL.UseProgram(ShaderManager._terrainShaderProgram);
            GL.UniformMatrix4(ShaderManager._terrainUViewLoc, false, ref view);
            GL.UniformMatrix4(ShaderManager._terrainUProjLoc, false, ref proj);
            GL.Uniform1(ShaderManager._textureUniformLocation, 0); // Bind to texture unit 0

            TextureManager.BindTextureArray(0); // Ensure the texture array is bound

            foreach (var terrain in LoadedTerrains.Values)
            {
                // Create a mirroring transformation for the Z-axis
                Matrix4 mirrorZ = Matrix4.CreateScale(1, 1, -1);
                // Matrix4 mirrorZ2 = mirrorZ * Matrix4.CreateScale(1, 1, -1);

                Matrix4 model = mirrorZ * Matrix4.CreateTranslation(terrain.Position);
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
    }
}