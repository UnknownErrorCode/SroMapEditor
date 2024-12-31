using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace SimpleGridFly
{
    public class SroMapProgram : GameWindow
    {
        private Camera _camera;
        private SroMapManager _mapManager;
        private int _shaderProgram;
        private int _uViewLoc;
        private int _uProjLoc;
        private int _uModelLoc;

        public SroMapProgram(GameWindowSettings gws, NativeWindowSettings nws) : base(gws, nws)
        {
            CursorState = CursorState.Normal;
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            // OpenGL Settings
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            // Initialize Camera
            _camera = new Camera(
                new Vector3(9600f, 500f, 9600f), // Start position
                225f, // Initial yaw
                -30f, // Initial pitch
                Size.X / (float)Size.Y);

            // Compile Shader Program
            _shaderProgram = CompileShaders();
            _uViewLoc = GL.GetUniformLocation(_shaderProgram, "uView");
            _uProjLoc = GL.GetUniformLocation(_shaderProgram, "uProjection");
            _uModelLoc = GL.GetUniformLocation(_shaderProgram, "uModel");

            // Initialize Map Manager
            _mapManager = new SroMapManager(_shaderProgram, _uModelLoc);

            // Load Map Data
            string mapRootDirectory = "path/to/your/map/data"; // Adjust path to your map data directory
            _mapManager.IndexTerrains(mapRootDirectory);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if (KeyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Escape))
                Close();

            // Process camera movement
            _camera.ProcessKeyboard(KeyboardState, (float)args.Time);

            // Update loaded terrains based on the camera's position
            _mapManager.UpdateLoadedTerrains(_camera.Position);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Use shader program
            GL.UseProgram(_shaderProgram);

            // Update view and projection matrices
            Matrix4 view = _camera.GetViewMatrix();
            Matrix4 projection = _camera.GetProjectionMatrix();
            GL.UniformMatrix4(_uViewLoc, false, ref view);
            GL.UniformMatrix4(_uProjLoc, false, ref projection);

            // Render map terrains
            _mapManager.RenderTerrains();

            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);
            _camera.UpdateAspectRatio(e.Width / (float)e.Height);
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            // Clean up resources
            _mapManager.Cleanup();
            GL.DeleteProgram(_shaderProgram);
        }

        private int CompileShaders()
        {
            // Vertex Shader Source
            string vertexShaderSource = @"#version 330 core
            layout(location = 0) in vec3 aPosition;
            layout(location = 1) in vec3 aNormal;
            layout(location = 2) in vec3 aColor;

            out vec3 vColor;

            uniform mat4 uModel;
            uniform mat4 uView;
            uniform mat4 uProjection;

            void main()
            {
                vColor = aColor;
                gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
            }";

            // Fragment Shader Source
            string fragmentShaderSource = @"#version 330 core
            in vec3 vColor;
            out vec4 FragColor;

            void main()
            {
                FragColor = vec4(vColor, 1.0);
            }";

            // Compile Vertex Shader
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderSource);
            GL.CompileShader(vertexShader);
            CheckShaderCompile(vertexShader);

            // Compile Fragment Shader
            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentShaderSource);
            GL.CompileShader(fragmentShader);
            CheckShaderCompile(fragmentShader);

            // Link Shaders into a Program
            int shaderProgram = GL.CreateProgram();
            GL.AttachShader(shaderProgram, vertexShader);
            GL.AttachShader(shaderProgram, fragmentShader);
            GL.LinkProgram(shaderProgram);
            CheckProgramLink(shaderProgram);

            // Delete shaders after linking
            GL.DetachShader(shaderProgram, vertexShader);
            GL.DetachShader(shaderProgram, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            return shaderProgram;
        }

        private void CheckShaderCompile(int shader)
        {
            GL.GetShader(shader, ShaderParameter.CompileStatus, out int status);
            if (status == 0)
            {
                string infoLog = GL.GetShaderInfoLog(shader);
                Console.WriteLine($"Shader compile error: {infoLog}");
            }
        }

        private void CheckProgramLink(int program)
        {
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int status);
            if (status == 0)
            {
                string infoLog = GL.GetProgramInfoLog(program);
                Console.WriteLine($"Program link error: {infoLog}");
            }
        }

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            var gws = GameWindowSettings.Default;
            var nws = new NativeWindowSettings
            {
                Size = new Vector2i(1280, 720),
                Title = "Silkroad Map Viewer"
            };

            using var program = new GridGame(gws, nws);
            program.Run();
        }
    }
}