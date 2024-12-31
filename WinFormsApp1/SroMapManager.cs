using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using WinFormsApp1;

namespace SimpleGridFly
{
    public class SroMapManager
    {
        public struct TerrainMesh
        {
            public int Vao;
            public int Vbo;
            public int VertexCount;
            public Vector3 Position;
        }

        private readonly int _shaderProgram;
        private readonly int _uModelLoc;

        public Dictionary<(int X, int Z), TerrainMesh> LoadedTerrains { get; private set; } = new();

        private readonly List<(int X, int Z, string FilePath)> _allTerrains = new();

        private readonly int _loadRange = 2;
        public float RegionSize { get; private set; } = 1920f;
        private float _cellScale = 7.5f;

        public SroMapManager(int shaderProgram, int modelLocation)
        {
            _shaderProgram = shaderProgram;
            _uModelLoc = modelLocation;
        }

        public void IndexTerrains(string rootDirectory)
        {
            if (!Directory.Exists(rootDirectory))
            {
                Console.WriteLine($"Directory does not exist: {rootDirectory}");
                return;
            }

            _allTerrains.Clear();

            var zDirectories = Directory.GetDirectories(rootDirectory);
            foreach (var zDir in zDirectories)
            {
                if (!int.TryParse(Path.GetFileName(zDir), out int zCoord)) continue;
                var mFiles = Directory.GetFiles(zDir, "*.m");

                foreach (var mFile in mFiles)
                {
                    if (!int.TryParse(Path.GetFileNameWithoutExtension(mFile), out int xCoord)) continue;
                    _allTerrains.Add((xCoord, zCoord, mFile));
                }
            }

            Console.WriteLine($"Indexed {_allTerrains.Count} terrain regions.");
        }

        public void UpdateLoadedTerrains(Vector3 cameraPosition)
        {
            int currentRegionX = (int)Math.Floor(cameraPosition.X / RegionSize);
            int currentRegionZ = (int)Math.Floor(cameraPosition.Z / RegionSize);

            int minX = currentRegionX - _loadRange;
            int maxX = currentRegionX + _loadRange;
            int minZ = currentRegionZ - _loadRange;
            int maxZ = currentRegionZ + _loadRange;

            HashSet<(int X, int Z)> regionsToLoad = new();
            for (int x = minX; x <= maxX; x++)
            {
                for (int z = minZ; z <= maxZ; z++)
                {
                    regionsToLoad.Add((x, z));
                }
            }

            foreach (var region in regionsToLoad)
            {
                if (!LoadedTerrains.ContainsKey(region))
                {
                    var terrain = _allTerrains.Find(t => t.X == region.X && t.Z == region.Z);
                    if (terrain != default)
                    {
                        LoadTerrain(region.X, region.Z, terrain.FilePath);
                    }
                }
            }

            var loadedRegions = new List<(int X, int Z)>(LoadedTerrains.Keys);
            foreach (var loadedRegion in loadedRegions)
            {
                if (!regionsToLoad.Contains(loadedRegion))
                {
                    UnloadTerrain(loadedRegion.X, loadedRegion.Z);
                }
            }
        }

        private void LoadTerrain(int regionX, int regionZ, string filePath)
        {
            try
            {
                byte[] fileData = File.ReadAllBytes(filePath);
                var region = new JMXmFile(fileData, (byte)regionX, (byte)regionZ);
                if (!region.Initialized) return;

                float[] terrainMesh = BuildRegionMesh(region);
                int vertexCount = terrainMesh.Length / 9;

                int vao = GL.GenVertexArray();
                int vbo = GL.GenBuffer();

                GL.BindVertexArray(vao);
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
                GL.BufferData(BufferTarget.ArrayBuffer, terrainMesh.Length * sizeof(float), terrainMesh, BufferUsageHint.StaticDraw);

                int stride = 9 * sizeof(float);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
                GL.EnableVertexAttribArray(0);
                GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));
                GL.EnableVertexAttribArray(1);
                GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, stride, 6 * sizeof(float));
                GL.EnableVertexAttribArray(2);

                GL.BindVertexArray(0);

                LoadedTerrains.Add((regionX, regionZ), new TerrainMesh
                {
                    Vao = vao,
                    Vbo = vbo,
                    VertexCount = vertexCount,
                    Position = CalculateRegionPosition(regionX, regionZ)
                });

                Console.WriteLine($"Loaded terrain at Region X:{regionX}, Z:{regionZ}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading terrain {filePath}: {ex.Message}");
            }
        }

        private Vector3 CalculateRegionPosition(int regionX, int regionZ)
        {
            return new Vector3(regionX * RegionSize, 0, regionZ * RegionSize);
        }

        private float[] BuildRegionMesh(JMXmFile region)
        {
            List<float> vertices = new();

            foreach (var block in region.Blocks.Values)
            {
                foreach (var cell in block.MapCells)
                {
                    var position = new Vector3(cell.Key.X * _cellScale, cell.Value.Height, cell.Key.Y * _cellScale);
                    var color = new Vector3(0.5f, 1f, 0.5f);
                    AddVertex(vertices, position, Vector3.UnitY, color);
                }
            }

            return vertices.ToArray();
        }

        private void AddVertex(List<float> list, Vector3 pos, Vector3 normal, Vector3 color)
        {
            list.Add(pos.X);
            list.Add(pos.Y);
            list.Add(pos.Z);
            list.Add(normal.X);
            list.Add(normal.Y);
            list.Add(normal.Z);
            list.Add(color.X);
            list.Add(color.Y);
            list.Add(color.Z);
        }

        private void UnloadTerrain(int regionX, int regionZ)
        {
            if (LoadedTerrains.TryGetValue((regionX, regionZ), out var tm))
            {
                GL.DeleteBuffer(tm.Vbo);
                GL.DeleteVertexArray(tm.Vao);
                LoadedTerrains.Remove((regionX, regionZ));
            }
        }

        public void RenderTerrains()
        {
            foreach (var terrain in LoadedTerrains.Values)
            {
                var model = Matrix4.CreateTranslation(terrain.Position);
                GL.UniformMatrix4(_uModelLoc, false, ref model);
                GL.BindVertexArray(terrain.Vao);
                GL.DrawArrays(PrimitiveType.Triangles, 0, terrain.VertexCount);
            }
        }

        public void Cleanup()
        {
            foreach (var terrain in LoadedTerrains.Values)
            {
                GL.DeleteBuffer(terrain.Vbo);
                GL.DeleteVertexArray(terrain.Vao);
            }
            LoadedTerrains.Clear();
            Console.WriteLine("All terrains have been unloaded and resources cleaned up.");
        }
    }
}