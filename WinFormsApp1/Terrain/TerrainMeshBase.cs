using OpenTK.Mathematics;

namespace SimpleGridFly
{
    public struct TerrainMeshBase
    {
        public Vector3 Position; // World position based on X and Z

        public int Vao;
        public int Vbo;
        public int VertexCount;
    }
}