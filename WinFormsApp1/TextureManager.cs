using OpenTK.Graphics.OpenGL4;
using Structs;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;

namespace SimpleGridFly
{
    internal static class TextureManager
    {
        private static Dictionary<int, string> TexturePaths = new(); // Maps texture IDs to file paths
        private static Dictionary<int, int> TextureMap = new(); // Maps texture IDs to OpenGL texture IDs
        private static int TextureArrayId; // OpenGL ID for the texture array

        private const int TextureWidth = 512; // Adjust to your texture resolution
        private const int TextureHeight = 512;



        /// <summary>
        /// Initializes the texture manager by loading textures from the given map folder.
        /// </summary>
        public static void InitializeTextures(string mapFolderPath)
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
                TexturePaths[texture.Id] = texturePath;
            }

            LoadTextureArray();
            MessageBox.Show($"Loaded {TextureMap.Count} textures into the texture array.");
        }

        /// <summary>
        /// Loads a texture into OpenGL and returns the texture ID.
        /// </summary>
        /// <param name="texturePath">The file path of the texture.</param>
        /// <returns>The OpenGL texture ID, or -1 if the texture failed to load.</returns>
        private static int LoadTexture(string texturePath)
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
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.BindTexture(TextureTarget.Texture2D, 0);
            return textureId;
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
        /// Creates a texture array and loads all textures into it.
        /// </summary>
        private static void LoadTextureArray()
        {
            int textureCount = TexturePaths.Count;
            TextureArrayId = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2DArray, TextureArrayId);

            // Allocate storage for the texture array
            GL.TexImage3D(TextureTarget.Texture2DArray, 0, PixelInternalFormat.Rgba,
                          TextureWidth, TextureHeight, textureCount, 0,
                          OpenTK.Graphics.OpenGL4.PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);

            // Load each texture into the array
            int layer = 0;
            foreach (var kvp in TexturePaths)
            {
                string texturePath = kvp.Value;

                if (File.Exists(texturePath))
                {

                    using Bitmap bitmap = new Bitmap(texturePath);
                    BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                                                      ImageLockMode.ReadOnly,
                                                      System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    GL.TexSubImage3D(TextureTarget.Texture2DArray, 0, 0, 0, layer, bitmap.Width, bitmap.Height, 1,
                                     OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

                    var err = GL.GetError();
                    bitmap.UnlockBits(data);

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
        /// Binds the texture array for use in rendering.
        /// </summary>
        public static void BindTextureArray(int textureUnit = 0)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + textureUnit);
            GL.BindTexture(TextureTarget.Texture2DArray, TextureArrayId);
        }

        /// <summary>
        /// Retrieves the texture array layer index for a given game-specific texture ID.
        /// </summary>
        public static bool TryGetTextureLayer(int textureId, out int layerIndex)
        {
            return TextureMap.TryGetValue(textureId, out layerIndex);
        }
    }
}