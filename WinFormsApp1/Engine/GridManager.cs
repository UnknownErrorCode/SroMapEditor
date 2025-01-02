using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SimpleGridFly;

public class GridManager
{
    // OpenGL handles for the grid: VAO (Vertex Array Object) and VBO (Vertex Buffer Object)
    private int _gridVao;

    private int _gridVbo;

    // Total number of vertices in the grid (used for rendering)
    private int _gridVertexCount;

    // Grid data: vertices for rendering the grid
    private float[] _gridVertices;

    public GridManager()
    {
        // Setup the grid with a specified number of regions
        int regionCountX = 255; // Number of regions along the X-axis
        int regionCountZ = 128; // Number of regions along the Z-axis
        _gridVertices = GenerateGrid(regionCountX, regionCountZ);
        _gridVertexCount = _gridVertices.Length / 6; // Each vertex contains 6 floats (position + color)

        // Configure VAO and VBO for grid rendering
        _gridVao = GL.GenVertexArray();
        _gridVbo = GL.GenBuffer();
        GL.BindVertexArray(_gridVao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _gridVbo);
        GL.BufferData(BufferTarget.ArrayBuffer, _gridVertices.Length * sizeof(float), _gridVertices, BufferUsageHint.StaticDraw);

        // Set up position attribute (location=0)
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        // Set up color attribute (location=1)
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);
        GL.BindVertexArray(0);
    }

    /// <summary>
    /// Generates a grid of lines on the X-Z plane, aligned with region boundaries.
    /// - **Major lines**: Placed at region boundaries (every 1920 units), with a distinct red color.
    /// - **Minor lines**: Subdivisions within regions (every 120 units), with a distinct blue color.
    /// Each line is represented as a pair of vertices in the format: (x, y, z, r, g, b).
    ///
    /// This method is useful for visualizing regions and their subdivisions in a grid layout.
    /// </summary>
    /// <param name="regionCountX">Number of regions along the X-axis.</param>
    /// <param name="regionCountZ">Number of regions along the Z-axis.</param>
    /// <returns>
    /// A flat array of floats representing the grid vertices and their colors.
    /// Each vertex is represented by 6 values: (x, y, z, r, g, b).
    /// </returns>
    public static float[] GenerateGrid(int regionCountX, int regionCountZ)
    {
        var lines = new List<float>();

        float regionSeparation = 1920f;  // Distance between major region boundaries.
        float blockSize = 120f;         // Distance between minor subdivisions within a region.
        float totalWidth = regionCountX * regionSeparation;
        float totalHeight = regionCountZ * regionSeparation;

        Vector3 minorColor = new Vector3(0.0f, 0.0f, 0.9f); // Blue for minor lines.
        Vector3 majorColor = new Vector3(0.9f, 0.0f, 0.0f); // Red for major lines.

        // Generate lines parallel to the X-axis (constant Z)
        for (int z = 0; z <= regionCountZ; z++)
        {
            float zPos = totalHeight - z * regionSeparation; // Invert Z-axis direction.

            lines.AddRange(new float[]
            {
            0, 0, zPos, majorColor.X, majorColor.Y, majorColor.Z,
            totalWidth, 0, zPos, majorColor.X, majorColor.Y, majorColor.Z,
            });

            if (z < regionCountZ)
            {
                for (int b = 1; b < 16; b++)
                {
                    float minorZ = zPos - b * blockSize; // Invert Z-axis direction.
                    lines.AddRange(new float[]
                    {
                    0, 0, minorZ, minorColor.X, minorColor.Y, minorColor.Z,
                    totalWidth, 0, minorZ, minorColor.X, minorColor.Y, minorColor.Z,
                    });
                }
            }
        }

        // Generate lines parallel to the Z-axis (constant X)
        for (int x = 0; x <= regionCountX; x++)
        {
            float xPos = x * regionSeparation;

            lines.AddRange(new float[]
            {
            xPos, 0, totalHeight, majorColor.X, majorColor.Y, majorColor.Z, // Start at the top.
            xPos, 0, 0, majorColor.X, majorColor.Y, majorColor.Z,           // End at the bottom.
            });

            if (x < regionCountX)
            {
                for (int b = 1; b < 16; b++)
                {
                    float minorX = xPos + b * blockSize;
                    lines.AddRange(new float[]
                    {
                    minorX, 0, totalHeight, minorColor.X, minorColor.Y, minorColor.Z,
                    minorX, 0, 0, minorColor.X, minorColor.Y, minorColor.Z,
                    });
                }
            }
        }

        return lines.ToArray();
    }

    internal void Cleanup()
    {
        GL.DeleteBuffer(_gridVbo);
        GL.DeleteVertexArray(_gridVao);
    }

    internal void RenderGrid(Matrix4 view, Matrix4 proj)
    {
        // Render the grid using the grid shader
        GL.UseProgram(ShaderManager._gridShaderProgram);

        GL.UniformMatrix4(ShaderManager._gridUViewLoc, false, ref view);
        GL.UniformMatrix4(ShaderManager._gridUProjLoc, false, ref proj);
        Matrix4 modelGrid = Matrix4.Identity;
        GL.UniformMatrix4(ShaderManager._gridUModelLoc, false, ref modelGrid);
        GL.BindVertexArray(_gridVao);
        GL.DrawArrays(PrimitiveType.Lines, 0, _gridVertexCount);
    }
}