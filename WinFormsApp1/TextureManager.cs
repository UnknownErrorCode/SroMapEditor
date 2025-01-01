using OpenTK.Graphics.OpenGL4;
using Structs;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;

namespace SimpleGridFly
{
    internal class TextureManager
    {
        private string[] TexPaths; // Array to store texture file paths
        private Dictionary<int, int> TextureMap = new(); // Maps texture indices to OpenGL texture IDs

        public TextureManager()
        {
        }

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
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.BindTexture(TextureTarget.Texture2D, 0);
            return textureId;
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

        internal bool TryGetGLTextureBySroTextureID(int textureIndex, out int textureId)
        {
            return TextureMap.TryGetValue(textureIndex, out textureId);
        }
    }
}