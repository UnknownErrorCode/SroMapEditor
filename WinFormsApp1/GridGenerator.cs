using OpenTK.Mathematics;

public static class GridGenerator
{
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
        var lines = new List<float>(); // Stores the grid vertices and their colors.

        // Constants for grid configuration
        float regionSeparation = 1920f;  // Distance between major region boundaries.
        float blockSize = 120f;         // Distance between minor subdivisions within a region.
        float totalWidth = regionCountX * regionSeparation; // Total grid width in the X direction.
        float totalHeight = regionCountZ * regionSeparation; // Total grid height in the Z direction.

        // Colors for the grid lines
        Vector3 minorColor = new Vector3(0.0f, 0.0f, 0.9f); // Blue color for minor lines.
        Vector3 majorColor = new Vector3(0.9f, 0.0f, 0.0f); // Red color for major lines.

        // Generate lines parallel to the X-axis (constant Z)
        for (int z = 0; z <= regionCountZ; z++)
        {
            float zPos = z * regionSeparation; // Position of the major line along the Z-axis.

            // Add a major line at the region boundary
            lines.AddRange(new float[]
            {
                0, 0, zPos, majorColor.X, majorColor.Y, majorColor.Z,             // Start vertex
                totalWidth, 0, zPos, majorColor.X, majorColor.Y, majorColor.Z,    // End vertex
            });

            // Add minor lines within the region
            if (z < regionCountZ) // Only add minor lines if not at the last region boundary
            {
                for (int b = 1; b < 16; b++) // 16 blocks per region
                {
                    float minorZ = zPos + b * blockSize; // Position of the minor line within the region.
                    lines.AddRange(new float[]
                    {
                        0, 0, minorZ, minorColor.X, minorColor.Y, minorColor.Z,             // Start vertex
                        totalWidth, 0, minorZ, minorColor.X, minorColor.Y, minorColor.Z,    // End vertex
                    });
                }
            }
        }

        // Generate lines parallel to the Z-axis (constant X)
        for (int x = 0; x <= regionCountX; x++)
        {
            float xPos = x * regionSeparation; // Position of the major line along the X-axis.

            // Add a major line at the region boundary
            lines.AddRange(new float[]
            {
                xPos, 0, 0, majorColor.X, majorColor.Y, majorColor.Z,             // Start vertex
                xPos, 0, totalHeight, majorColor.X, majorColor.Y, majorColor.Z,   // End vertex
            });

            // Add minor lines within the region
            if (x < regionCountX) // Only add minor lines if not at the last region boundary
            {
                for (int b = 1; b < 16; b++) // 16 blocks per region
                {
                    float minorX = xPos + b * blockSize; // Position of the minor line within the region.
                    lines.AddRange(new float[]
                    {
                        minorX, 0, 0, minorColor.X, minorColor.Y, minorColor.Z,             // Start vertex
                        minorX, 0, totalHeight, minorColor.X, minorColor.Y, minorColor.Z,  // End vertex
                    });
                }
            }
        }

        // Return the grid data as a flat array
        return lines.ToArray();
    }
}