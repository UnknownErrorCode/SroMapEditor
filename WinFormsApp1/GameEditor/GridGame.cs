// GridGame.cs
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SimpleGridFly.Texture;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Keys = OpenTK.Windowing.GraphicsLibraryFramework.Keys;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace SimpleGridFly
{
    public class GridGame : GameWindow
    {
        // --- Member Variables ---

        // Interval (in seconds) for terrain updates
        private const double UpdateInterval = 0.5;

        // Camera for viewing and navigating the scene
        private Camera _camera;

        // Current region coordinates based on camera position
        private int _currentRegionX = 0;

        private int _currentRegionZ = 0;

        // The current text to display on screen
        private string _currentText = string.Empty;

        // Flags and variables for mouse dragging
        private bool _isDragging = false;

        private Vector2 _lastMousePos;

        // Terrain manager for handling terrain loading, rendering, and texture management
        private TerrainManager _terrainManager;

        private GridManager _gridManager;

        // Cache for pre-generated text textures to avoid redundant processing
        private Dictionary<string, int> _textTexturesCache = new Dictionary<string, int>();

        // Timer for regulating terrain updates
        private double _timeSinceLastUpdate = 0.0;

        public List<(int X, int Z, string)> LoadedTerrains => _terrainManager.AllTerrains;

        public GridGame(GameWindowSettings gws, NativeWindowSettings nws)
            : base(gws, nws)
        {
            // We do NOT lock the mouse, so we can click & drag
            CursorState = CursorState.Normal;
            // Set update frequency to 50 frames per second
            UpdateFrequency = 50f;
            // Initialize camera near middle of the grid, a bit above, looking forward
            _camera = new Camera(
                new Vector3(167200f, 500f, 167200f), // Position at the center (assuming 10x10 regions)
                225f,                             // Yaw to face towards negative Z and X
                -30f,                             // Pitch to look downward
                Size.X / (float)Size.Y            // Aspect ratio
            );
            ShaderManager.BuildShader();

            _gridManager = new GridManager();
            _terrainManager = new TerrainManager();
        }

        internal void InitializeTerrainMeshes()
           => _terrainManager.IndexTerrains("I:\\Clients\\Exay-Origin V1.014\\Map");

        protected override void OnLoad()
        {
            base.OnLoad();

            // Setup OpenGL states and enable debugging
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.DebugOutput);
            GL.DebugMessageCallback(DebugCallback, IntPtr.Zero);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            // Start dragging if the left mouse button is pressed
            if (e.Button == MouseButton.Left)
            {
                _isDragging = true;
                _lastMousePos = MousePosition;
            }
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);

            if (_isDragging)
            {
                // Calculate delta
                float deltaX = e.Position.X - _lastMousePos.X;
                float deltaY = e.Position.Y - _lastMousePos.Y;
                _lastMousePos = e.Position;

                // Update camera yaw/pitch
                _camera.ProcessMouseDrag(deltaX, deltaY);
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            // Stop dragging if the left mouse button is released
            if (e.Button == MouseButton.Left)
            {
                _isDragging = false;
            }
        }

        /// <summary>
        /// Handles rendering the current frame.
        /// This includes clearing the screen, rendering the grid, drawing terrains, and displaying a text overlay with the current region and camera position.
        /// </summary>
        /// <param name="args">Arguments containing timing information for the frame.</param>
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            // Clear the screen and prepare for rendering
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            var view = _camera.GetViewMatrix();
            var proj = _camera.GetProjectionMatrix();

            _gridManager.RenderGrid(view, proj);

            _terrainManager.RenderTerrains(view, proj);

            // Render text overlay (current region and camera position)
            if (!string.IsNullOrEmpty(_currentText))
            {
                RenderText(_currentText, new Vector2(10, 10));
            }

            // Swap buffers to display the frame
            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            // Adjust viewport and update camera aspect ratio on window resize
            GL.Viewport(0, 0, e.Width, e.Height);
            _camera.UpdateAspectRatio(e.Width / (float)e.Height);
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            // Cleanup grid
            _gridManager.Cleanup();

            // Cleanup terrains
            _terrainManager.Cleanup();

            // Cleanup text
            CleanupTextTextures();

            ShaderManager.Cleanup();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            // Close the game if the escape key is pressed
            if (KeyboardState.IsKeyDown(Keys.Escape))
                Close();

            // Handle camera movement and keyboard inputs
            _camera.ProcessKeyboard(KeyboardState, (float)args.Time);

            // Update terrains periodically and handle region changes
            _timeSinceLastUpdate += args.Time;
            if (_timeSinceLastUpdate >= UpdateInterval)
            {
                _timeSinceLastUpdate = 0.0;
                _terrainManager.UpdateLoadedTerrains(_camera.Position);
            }

            // Press 'O' to open folder dialog
            if (KeyboardState.IsKeyPressed(Keys.O))
            {
                _terrainManager.IndexTerrains("I:\\Clients\\Exay-Origin V1.014\\Map");
            }

            if (KeyboardState.IsKeyPressed(Keys.T))
            {
                TextureManager.InitializeTextures("I:\\Clients\\Exay-Origin V1.014\\Map");
            }

            // Update current region coordinates
            int newRegionX = (int)Math.Floor(_camera.Position.X / _terrainManager.RegionSeparation);
            int newRegionZ = (int)Math.Floor(_camera.Position.Z / _terrainManager.RegionSeparation);

            // Check if region has changed
            if (newRegionX != _currentRegionX || newRegionZ != _currentRegionZ)
            {
                _currentRegionX = newRegionX;
                _currentRegionZ = newRegionZ;
                Console.WriteLine($"Current Region - X: {_currentRegionX}, Z: {_currentRegionZ}");
            }
            _currentText = $"Region X: {_currentRegionX}, Z: {_currentRegionZ}   {_camera.Position}  ";
        }

        private static void DebugCallback(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
        {
            string msg = Marshal.PtrToStringAnsi(message, length);
            Console.WriteLine($"GL Debug: {msg}");
        }

        /// <summary>
        /// Deletes all cached text textures to free resources.
        /// </summary>
        private void CleanupTextTextures()
        {
            foreach (var tex in _textTexturesCache.Values)
            {
                GL.DeleteTexture(tex);
            }
            _textTexturesCache.Clear();
        }

        /// <summary>
        /// Creates a texture from the provided text using System.Drawing.
        /// Caches textures to avoid recreating them every frame.
        /// </summary>
        private void CreateTextTexture(string text)
        {
            if (_textTexturesCache.ContainsKey(text))
                return; // Texture already exists

            // Create a bitmap
            Bitmap bmp = new Bitmap(512, 64);
            using (Graphics gfx = Graphics.FromImage(bmp))
            {
                gfx.Clear(System.Drawing.Color.Transparent);
                using (Font font = new Font(FontFamily.GenericSansSerif, 16))
                using (SolidBrush brush = new SolidBrush(System.Drawing.Color.White))
                {
                    gfx.DrawString(text, font, brush, new PointF(0, 0));
                }
            }

            // Generate OpenGL texture
            int texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texture);

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                                           ImageLockMode.ReadOnly,
                                           System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D,
                          0,
                          PixelInternalFormat.Rgba,
                          data.Width,
                          data.Height,
                          0,
                          PixelFormat.Bgra,
                          PixelType.UnsignedByte,
                          data.Scan0);

            bmp.UnlockBits(data);
            bmp.Dispose();

            // Texture parameters
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            // Store the texture in the cache
            _textTexturesCache.Add(text, texture);
        }

        /// <summary>
        /// Renders text by mapping the texture onto a quad.
        /// </summary>
        private void RenderText(string text, Vector2 position)
        {
            if (!_textTexturesCache.ContainsKey(text))
            {
                CreateTextTexture(text);
            }

            int texture = _textTexturesCache[text];

            // Use text shader program
            GL.UseProgram(ShaderManager._textShaderProgram);

            // Create model matrix to position the text
            Matrix4 model = Matrix4.CreateTranslation(new Vector3(position.X, position.Y, 0.0f));
            GL.UniformMatrix4(GL.GetUniformLocation(ShaderManager._textShaderProgram, "uModel"), false, ref model);

            // Create orthographic projection matrix for 2D rendering
            Matrix4 ortho = Matrix4.CreateOrthographicOffCenter(0, Size.X, Size.Y, 0, -1.0f, 1.0f);
            GL.UniformMatrix4(GL.GetUniformLocation(ShaderManager._textShaderProgram, "uProjection"), false, ref ortho);

            // Set view matrix to identity for 2D overlay
            Matrix4 view = Matrix4.Identity;
            GL.UniformMatrix4(GL.GetUniformLocation(ShaderManager._textShaderProgram, "uView"), false, ref view);

            // Enable blending
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            // Bind texture
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.Uniform1(GL.GetUniformLocation(ShaderManager._textShaderProgram, "uTextTexture"), 0);

            // Bind VAO and draw
            GL.BindVertexArray(ShaderManager._textVao);
            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);

            // Cleanup
            GL.BindVertexArray(0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.UseProgram(0);

            // Disable blending
            GL.Disable(EnableCap.Blend);
        }
    }
}