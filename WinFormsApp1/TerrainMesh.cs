using OpenTK.Mathematics;
using Structs;
using WinFormsApp1;

namespace SimpleGridFly
{
    internal class TerrainMesh
    {
        public virtual float[] Vertices { get; private set; }

        public TerrainMesh(JMXmFile region)
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

                        if (TextureManager.TryGetTextureLayer(textureIndex, out int openGLTexID))
                            textureIndex = openGLTexID;

                        float h0 = meshCell0.Height;
                        float h1 = meshCell1.Height;
                        float h2 = meshCell2.Height;
                        float h3 = meshCell3.Height;

                        // Invert Z-axis by negating the Z-coordinate
                        Vector3 p0 = new Vector3(baseX + cellX * CELL_SIZE, h0, -(baseZ + cellZ * CELL_SIZE));
                        Vector3 p1 = new Vector3(baseX + (cellX + 1) * CELL_SIZE, h1, -(baseZ + cellZ * CELL_SIZE));
                        Vector3 p2 = new Vector3(baseX + cellX * CELL_SIZE, h2, -(baseZ + (cellZ + 1) * CELL_SIZE));
                        Vector3 p3 = new Vector3(baseX + (cellX + 1) * CELL_SIZE, h3, -(baseZ + (cellZ + 1) * CELL_SIZE));

                        Vector3 n0 = ComputeNormal(p0, p1, p2);
                        Vector3 n1 = ComputeNormal(p2, p1, p3);

                        Vector2 uv0 = new Vector2(0.0f, 0.0f);
                        Vector2 uv1 = new Vector2(1.0f, 0.0f);
                        Vector2 uv2 = new Vector2(0.0f, 1.0f);
                        Vector2 uv3 = new Vector2(1.0f, 1.0f);

                        // Add vertices for two triangles
                        AddVertexWithTexture(vertices, p0, n0, uv0, textureIndex);
                        AddVertexWithTexture(vertices, p1, n0, uv1, textureIndex);
                        AddVertexWithTexture(vertices, p2, n0, uv2, textureIndex);

                        AddVertexWithTexture(vertices, p2, n1, uv2, textureIndex);
                        AddVertexWithTexture(vertices, p1, n1, uv1, textureIndex);
                        AddVertexWithTexture(vertices, p3, n1, uv3, textureIndex);
                    }
                }
            }
            Vertices = vertices.ToArray();
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

        private void AddVertexWithTexture(List<float> vertices, Vector3 pos, Vector3 normal, Vector2 texCoord, int textureId = 0)
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
    }
}