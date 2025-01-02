using OpenTK.Graphics.OpenGL4;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using ErrorCode = OpenTK.Graphics.OpenGL4.ErrorCode;

namespace SimpleGridFly.Texture
{
    internal static class TextureManager
    {
        #region Fields

        private const int TextureHeight = 512;
        private const int TextureWidth = 512;
        private static int TextureArrayId;
        private static Dictionary<int, int> TextureMap = new();
        private static Dictionary<int, string> TexturePaths = new();

        private static string directory = string.Empty;

        #endregion Fields

        internal static Dictionary<int, string> TextureNames => TexturePaths;

        public static bool Initialized { get; private set; } = false;

        #region Methods

        /// <summary>
        /// Binds the texture array for use in rendering.
        /// </summary>
        public static void BindTextureArray(int textureUnit = 0)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + textureUnit);
            GL.BindTexture(TextureTarget.Texture2DArray, TextureArrayId);
        }

        // Adjust to your texture resolution
        /// <summary>
        /// Initializes the texture manager by loading textures from the given map folder.
        /// </summary>
        public static void InitializeTextures(string mapFolderPath)
        {
            directory = mapFolderPath;
            if (Initialized)
            {
                ReloadTextures();
                return;
            }
            LoadTexturePaths(mapFolderPath);

            LoadTextureArray();
            Initialized = true;
            MessageBox.Show($"Loaded {TextureMap.Count} textures into the texture array.");
        }

        private static void LoadTexturePaths(string mapFolderPath)
        {
            string ifoPath = $"{mapFolderPath}\\tile2d.ifo";

            TexturePaths.Clear();
            if (!File.Exists(ifoPath))
            {
                MessageBox.Show("Could not find tile2d.ifo. Terminating application.");
                return;
            }

            List<TextureInfo> textureInfos = ParseTextureFile(ifoPath);

            foreach (var texture in textureInfos)
            {
                string texturePath = $"{mapFolderPath}\\tile2d\\{texture.TexturePath}".Replace(".ddj", ".png");
                TexturePaths[texture.Id] = texturePath;
            }
        }

        /// <summary>
        /// Reloads the texture array, clearing existing textures and reloading them from the paths.
        /// </summary>
        public static void ReloadTextures()
        {
            // Delete the existing texture array
            if (TextureArrayId != 0)
            {
                GL.DeleteTexture(TextureArrayId);
                TextureArrayId = 0;
                TextureMap.Clear();
            }

            LoadTexturePaths(directory);

            // Reinitialize the texture array
            LoadTextureArray();
            Console.WriteLine($"Reloaded {TextureMap.Count} textures into the texture array.");
        }

        /// <summary>
        /// Parses the tile2d.ifo file and extracts texture information.
        /// </summary>
        public static List<TextureInfo> ParseTextureFile(string filePath)
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
                if (string.IsNullOrWhiteSpace(line)) continue;

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

        /// <summary>
        /// Retrieves the texture array layer index for a given game-specific texture ID.
        /// </summary>
        public static bool TryGetTextureLayer(int textureId, out int layerIndex)
        {
            return TextureMap.TryGetValue(textureId, out layerIndex);
        }

        /// <summary>
        /// Creates a texture array and loads all textures into it, resizing smaller textures to the required dimensions.
        /// </summary>
        private static void LoadTextureArray()
        {
            int textureCount = TexturePaths.Count;
            TextureArrayId = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2DArray, TextureArrayId);

            // Allocate storage for the texture array
            GL.TexImage3D(TextureTarget.Texture2DArray, 0, PixelInternalFormat.Rgba,
                          TextureWidth, TextureHeight, textureCount, 0,
                          OpenTK.Graphics.OpenGL4.PixelFormat.Rgba, PixelType.UnsignedByte, nint.Zero);

            // Load each texture into the array
            int layer = 0;
            foreach (var kvp in TexturePaths)
            {
                string texturePath = kvp.Value;

                if (File.Exists(texturePath))
                {
                    using Bitmap originalBitmap = new Bitmap(texturePath);

                    Bitmap resizedBitmap;
                    // Check if the texture size matches the required size
                    if (originalBitmap.Width != TextureWidth || originalBitmap.Height != TextureHeight)
                    {
                        Console.WriteLine($"Resizing texture: {texturePath} from {originalBitmap.Width}x{originalBitmap.Height} to {TextureWidth}x{TextureHeight}");
                        resizedBitmap = ResizeBitmap(originalBitmap, TextureWidth, TextureHeight);
                    }
                    else
                    {
                        resizedBitmap = new Bitmap(originalBitmap); // Clone the original if resizing isn't needed
                    }

                    // Lock the resized bitmap data
                    BitmapData data = resizedBitmap.LockBits(new Rectangle(0, 0, resizedBitmap.Width, resizedBitmap.Height),
                                                             ImageLockMode.ReadOnly,
                                                             System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    GL.TexSubImage3D(TextureTarget.Texture2DArray, 0, 0, 0, layer, resizedBitmap.Width, resizedBitmap.Height, 1,
                                     OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

                    ErrorCode err = GL.GetError();
                    if (err != ErrorCode.NoError)
                    {
                        Console.WriteLine($"OpenGL Error: {err} while uploading texture {texturePath}");
                    }

                    resizedBitmap.UnlockBits(data);
                    resizedBitmap.Dispose(); // Dispose of the resized bitmap after use

                    TextureMap[kvp.Key] = layer; // Map the texture ID to the array layer
                    layer++;
                }
                else
                {
                    Console.WriteLine($"Failed to load texture: {texturePath}");
                }
            }

            // Set texture parameters
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            GL.BindTexture(TextureTarget.Texture2DArray, 0);
        }

        /// <summary>
        /// Resizes a bitmap to the specified width and height.
        /// </summary>
        /// <param name="source">The source bitmap to resize.</param>
        /// <param name="width">The target width.</param>
        /// <param name="height">The target height.</param>
        /// <returns>A resized bitmap.</returns>
        private static Bitmap ResizeBitmap(Bitmap source, int width, int height)
        {
            Bitmap resized = new Bitmap(width, height);
            using Graphics graphics = Graphics.FromImage(resized);
            graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            graphics.DrawImage(source, 0, 0, width, height);
            return resized;
        }

        #endregion Methods
    }
}