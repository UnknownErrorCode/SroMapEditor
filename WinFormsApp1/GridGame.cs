// GridGame.cs
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Keys = OpenTK.Windowing.GraphicsLibraryFramework.Keys;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace SimpleGridFly
{
    public class GridGame : GameWindow
    {
        // --- Member Variables ---
        private Camera _camera;

        // Grid data
        private float[] _gridVertices;

        private int _gridVao;
        private int _gridVbo;
        private int _gridVertexCount;

        // Terrain Manager
        private TerrainManager _terrainManager;

        // Mouse drag
        private bool _isDragging = false;

        private Vector2 _lastMousePos;

        // Terrain root directory
        private string _terrainRootDirectory = string.Empty;

        // Timer to control update frequency
        private double _timeSinceLastUpdate = 0.0;

        private const double UpdateInterval = 0.5; // seconds

        // Current Region Coordinates
        private int _currentRegionX = 0;

        private int _currentRegionZ = 0;

        // Text rendering variables
        private int _textTexture;

        // Text management
        private string _currentText = string.Empty;

        private Dictionary<string, int> _textTexturesCache = new Dictionary<string, int>();

        public GridGame(GameWindowSettings gws, NativeWindowSettings nws)
            : base(gws, nws)
        {
            // We do NOT lock the mouse, so we can click & drag
            CursorState = CursorState.Normal;
            UpdateFrequency = 50f;

        }

        private static void DebugCallback(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
        {
            string msg = Marshal.PtrToStringAnsi(message, length);
            Console.WriteLine($"GL Debug: {msg}");
        }
        protected override void OnLoad()
        {
            base.OnLoad();

            // GL settings
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.DebugOutput);
            GL.DebugMessageCallback(DebugCallback, IntPtr.Zero);

            // Initialize camera near middle of the grid, a bit above, looking forward
            _camera = new Camera(
                new Vector3(167200f, 500f, 167200f), // Position at the center (assuming 10x10 regions)
                225f,                             // Yaw to face towards negative Z and X
                -30f,                             // Pitch to look downward
                Size.X / (float)Size.Y
            );

            // 1) Define how many regions you want in X and Z
            int regionCountX = 255; // Example: 10 regions along X-axis
            int regionCountZ = 128; // Example: 10 regions along Z-axis

            // 2) Generate the grid based on region counts
            _gridVertices = GridGenerator.GenerateGrid(regionCountX, regionCountZ);
            _gridVertexCount = _gridVertices.Length / 6; // each vertex has 6 floats

            // 3) Create & bind a VAO for grid
            _gridVao = GL.GenVertexArray();
            _gridVbo = GL.GenBuffer();

            GL.BindVertexArray(_gridVao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _gridVbo);
            GL.BufferData(BufferTarget.ArrayBuffer,
                          _gridVertices.Length * sizeof(float),
                          _gridVertices,
                          BufferUsageHint.StaticDraw);

            // Position attrib (location=0), 3 floats
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // Color attrib (location=1), 3 floats
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false,
                                   6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.BindVertexArray(0);

            ShaderManager.BuildShader();

            // 6) Initialize TerrainManager with terrain shader program and model uniform location
            _terrainManager = new TerrainManager(ShaderManager._terrainShaderProgram, ShaderManager._terrainUModelLoc);
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            // Cleanup grid
            GL.DeleteBuffer(_gridVbo);
            GL.DeleteVertexArray(_gridVao);

            // Cleanup terrains
            _terrainManager.Cleanup();

            // Cleanup text
            CleanupTextTextures();

            ShaderManager.Cleanup();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);
            _camera.UpdateAspectRatio(e.Width / (float)e.Height);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            // If left button pressed => start dragging
            if (e.Button == MouseButton.Left)
            {
                _isDragging = true;
                _lastMousePos = MousePosition;
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            // If left button released => stop dragging
            if (e.Button == MouseButton.Left)
            {
                _isDragging = false;
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

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if (KeyboardState.IsKeyDown(Keys.Escape))
                Close();

            // Process camera movement
            _camera.ProcessKeyboard(KeyboardState, (float)args.Time);

            // Press 'O' to open folder dialog
            if (KeyboardState.IsKeyPressed(Keys.O))
            {


                _terrainManager.IndexTerrains("I:\\Clients\\Exay-Origin V1.014\\Map");

            }

            if (KeyboardState.IsKeyPressed(Keys.T))
            {
                _terrainManager.InitializeTextures();
            }

            // Update terrains based on camera position at defined intervals
            _timeSinceLastUpdate += args.Time;
            if (_timeSinceLastUpdate >= UpdateInterval)
            {
                _timeSinceLastUpdate = 0.0;
                _terrainManager.UpdateLoadedTerrains(_camera.Position);
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
                // Update on-screen text
                _currentText = $"Region X: {_currentRegionX}, Z: {_currentRegionZ}";
            }
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            // Clear the screen
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // 1) Draw the grid with grid shader
            GL.UseProgram(ShaderManager._gridShaderProgram);

            // Send camera matrices
            var view = _camera.GetViewMatrix();
            var proj = _camera.GetProjectionMatrix();

            GL.UniformMatrix4(ShaderManager._gridUViewLoc, false, ref view);
            GL.UniformMatrix4(ShaderManager._gridUProjLoc, false, ref proj);

            // Model matrix for grid
            Matrix4 modelGrid = Matrix4.Identity;
            GL.UniformMatrix4(ShaderManager._gridUModelLoc, false, ref modelGrid);

            // Draw the grid
            GL.BindVertexArray(_gridVao);

            GL.DrawArrays(PrimitiveType.Lines, 0, _gridVertexCount);

            // 2) Draw all loaded terrains with terrain shader
            GL.UseProgram(ShaderManager._terrainShaderProgram);
            GL.UniformMatrix4(ShaderManager._terrainUViewLoc, false, ref view);
            GL.UniformMatrix4(ShaderManager._terrainUProjLoc, false, ref proj);
            GL.Uniform1(ShaderManager._textureUniformLocation, 0); // Bind to texture unit 0
            _terrainManager.RenderTerrains();

            // 3) Render the current region's coordinates as text
            if (!string.IsNullOrEmpty(_currentText))
            {
                RenderText(_currentText, new Vector2(10, 10));
            }

            SwapBuffers();
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
            Bitmap bmp = new Bitmap(256, 64);
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
    }
}