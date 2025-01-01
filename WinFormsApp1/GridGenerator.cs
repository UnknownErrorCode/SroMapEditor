using OpenTK.Mathematics;

public static class GridGenerator
{
    /// <summary>
    /// Generates a grid of lines on the X-Z plane, aligned with region boundaries.
    /// Major lines are at region boundaries (every 1920 units).
    /// Minor lines are within regions (every 120 units).
    /// Each line is stored as (x, y, z, r, g, b).
    /// </summary>
    public static float[] GenerateGrid(int regionCountX, int regionCountZ)
    {
        var lines = new List<float>();

        float regionSeparation = 1920f;
        float blockSize = 120f;
        float totalWidth = regionCountX * regionSeparation;
        float totalHeight = regionCountZ * regionSeparation;

        Vector3 minorColor = new Vector3(0.0f, 0.0f, 0.9f);
        Vector3 majorColor = new Vector3(0.9f, 0.0f, 0.0f);

        // Lines parallel to X (constant Z)
        for (int z = 0; z <= regionCountZ; z++)
        {
            float zPos = z * regionSeparation;
            Vector3 color = majorColor;

            // Major grid line at region boundary
            lines.AddRange(new float[]
            {
                0, 0, zPos, majorColor.X, majorColor.Y, majorColor.Z,
                totalWidth, 0, zPos, majorColor.X, majorColor.Y, majorColor.Z,
            });

            // Minor grid lines within the region
            for (int b = 1; b < 17; b++) // 16 blocks per region
            {
                float minorZ = zPos + b * blockSize;
                color = minorColor;
                lines.AddRange(new float[]
                {
                    0, 0, minorZ, color.X, color.Y, color.Z,
                    totalWidth, 0, minorZ, color.X, color.Y, color.Z,
                });
            }
        }

        // Lines parallel to Z (constant X)
        for (int x = 0; x <= regionCountX; x++)
        {
            float xPos = x * regionSeparation;
            Vector3 color = majorColor;

            // Major grid line at region boundary
            lines.AddRange(new float[]
            {
                xPos, 0, 0, color.X, color.Y, color.Z,
                xPos, 0, totalHeight, color.X, color.Y, color.Z,
            });

            // Minor grid lines within the region
            for (int b = 1; b < 17; b++) // 16 blocks per region
            {
                float minorX = xPos + b * blockSize;
                color = minorColor;
                lines.AddRange(new float[]
                {
                    minorX, 0, 0, color.X, color.Y, color.Z,
                    minorX, 0, totalHeight, color.X, color.Y, color.Z,
                });
            }
        }

        return lines.ToArray();
    }
}