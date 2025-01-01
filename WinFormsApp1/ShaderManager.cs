// GridGame.cs

using OpenTK.Graphics.OpenGL4;

namespace SimpleGridFly
{
    internal static class ShaderManager
    {
        // Grid Vertex Shader Source
        public const string GridVertexShaderSrc = @"
#version 330 core
layout (location=0) in vec3 aPosition;
layout (location=1) in vec3 aColor;

out vec3 vColor;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

void main()
{
    vColor = aColor;
    gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
}";

        // Grid Fragment Shader Source
        public const string GridFragmentShaderSrc = @"
#version 330 core
in vec3 vColor;
out vec4 FragColor;

void main()
{
    FragColor = vec4(vColor, 1.0);
}";

        // Terrain Vertex Shader Source
        public const string TerrainVertexShaderSrc = @"
#version 330 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoord;
layout (location = 3) in float aTexLayer;

out vec3 vNormal;
out vec2 vTexCoord;
flat out float vTexLayer;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

void main()
{
    vNormal = mat3(transpose(inverse(uModel))) * aNormal; //aNormal;
    vTexCoord = aTexCoord;
    vTexLayer = aTexLayer;

    gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
}

";

        // Terrain Fragment Shader Source
        public const string TerrainFragmentShaderSrc = @"
#version 330 core

in vec3 vNormal;
in vec2 vTexCoord;
flat in float vTexLayer;

out vec4 FragColor;

uniform sampler2DArray uTextureArray;

void main()
{
    vec3 lightDir = normalize(vec3(-0.2, -1.0, -0.3));
    vec3 ambientColor = vec3(0.3, 0.3, 0.3);
    vec3 lightColor = vec3(1.0, 1.0, 1.0);

    // Sample the correct layer of the texture array
    vec4 texColor = texture(uTextureArray, vec3(vTexCoord, vTexLayer));

    // Apply lighting
    vec3 norm = normalize(vNormal);
    float diff = max(dot(norm, -lightDir), 0.0);

    vec3 ambient = ambientColor * texColor.rgb;
    vec3 diffuse = diff * lightColor * texColor.rgb;

    vec3 finalColor = ambient + diffuse;
    FragColor = vec4(finalColor, texColor.a);
}

";

        // Terrain Vertex Shader Source
        public const string TerrainVertexShaderSrcOld = @"
#version 330 core
layout (location=0) in vec3 aPosition;
layout (location=1) in vec3 aNormal;
layout (location=2) in vec2 aTexCoord; // Add texture coordinates

out vec3 vNormal;
out vec3 vFragPos;
out vec2 vTexCoord; // Pass texture coordinates to fragment shader

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

void main()
{
    vFragPos = vec3(uModel * vec4(aPosition, 1.0));
    vNormal = mat3(transpose(inverse(uModel))) * aNormal;
    vTexCoord = aTexCoord; // Pass through
    gl_Position = uProjection * uView * vec4(vFragPos, 1.0);
}
";

        // Terrain Fragment Shader Source
        public const string TerrainFragmentShaderSrcOld = @"
#version 330 core
in vec3 vNormal;
in vec3 vFragPos;
in vec2 vTexCoord; // Receive texture coordinates

out vec4 FragColor;

uniform sampler2D uTexture; // Texture sampler

void main()
{
    // vec3 ambient = vec3(0.3) * texture(uTexture, vTexCoord).rgb; // Ambient with texture
    //FragColor = vec4(vTexCoord, 1.0, 1.0); // Visualize texture coordinates
    FragColor = texture(uTexture, vTexCoord);
}
";

        //FragColor = vec4(vTexCoord, 0.0, 1.0); // Visualize texture coordinates
        //FragColor = vec4(ambient, 1.0);
        //

        // Text Vertex Shader Source
        public const string TextVertexShaderSrc = @"
#version 330 core
layout (location=0) in vec3 aPosition;
layout (location=1) in vec2 aTexCoord;

out vec2 TexCoord;

uniform mat4 uProjection;
uniform mat4 uView;
uniform mat4 uModel;

void main()
{
    TexCoord = aTexCoord;
    gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
}";

        // Text Fragment Shader Source
        public const string TextFragmentShaderSrc = @"
#version 330 core
in vec2 TexCoord;
out vec4 FragColor;

uniform sampler2D uTextTexture;

void main()
{
    vec4 sampled = texture(uTextTexture, TexCoord);
    if (sampled.a < 0.1)
        discard;
    FragColor = sampled;
}";

        // Shaders
        public static int _terrainShaderProgram;

        public static int _terrainUViewLoc;
        public static int _terrainUProjLoc;
        public static int _terrainUModelLoc;
        public static int _textureUniformLocation;
        public static int _gridShaderProgram;
        public static int _gridUViewLoc;
        public static int _gridUProjLoc;
        public static int _gridUModelLoc;

        public static int _textVao;
        public static int _textVbo;
        public static int _textEbo;
        public static int _textShaderProgram;

        internal static void BuildShader()
        {
            // 4) Build grid shader
            _gridShaderProgram = CreateShaderProgram(ShaderManager.GridVertexShaderSrc, ShaderManager.GridFragmentShaderSrc);
            _gridUViewLoc = GL.GetUniformLocation(_gridShaderProgram, "uView");
            _gridUProjLoc = GL.GetUniformLocation(_gridShaderProgram, "uProjection");
            _gridUModelLoc = GL.GetUniformLocation(_gridShaderProgram, "uModel");

            // 5) Build terrain shader
            _terrainShaderProgram = CreateShaderProgram(ShaderManager.TerrainVertexShaderSrc, ShaderManager.TerrainFragmentShaderSrc);
            _terrainUViewLoc = GL.GetUniformLocation(_terrainShaderProgram, "uView");
            _terrainUProjLoc = GL.GetUniformLocation(_terrainShaderProgram, "uProjection");
            _terrainUModelLoc = GL.GetUniformLocation(_terrainShaderProgram, "uModel");
            _textureUniformLocation = GL.GetUniformLocation(_terrainShaderProgram, "uTextureArray");

            SetupTextQuad();
            CreateTextShader();
        }

        /// <summary>
        /// Sets up a quad for text rendering.
        /// </summary>
        private static void SetupTextQuad()
        {
            float[] quadVertices = {
                // positions      // texcoords
                0.0f, 0.0f, 0.0f, 0.0f, 0.0f, // bottom-left
                256.0f, 0.0f, 0.0f, 1.0f, 0.0f, // bottom-right
                256.0f, 64.0f, 0.0f, 1.0f, 1.0f, // top-right
                0.0f, 64.0f, 0.0f, 0.0f, 1.0f  // top-left
            };

            uint[] indices = {
                0, 1, 2,
                2, 3, 0
            };

            _textVao = GL.GenVertexArray();
            _textVbo = GL.GenBuffer();
            _textEbo = GL.GenBuffer();

            GL.BindVertexArray(_textVao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _textVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, quadVertices.Length * sizeof(float), quadVertices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _textEbo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            // Position attribute (location=0)
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // TexCoord attribute (location=1)
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.BindVertexArray(0);
        }

        /// <summary>
        /// Creates and compiles the text shader program.
        /// </summary>
        private static void CreateTextShader()
        {
            // Compile vertex shader
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, ShaderManager.TextVertexShaderSrc);
            GL.CompileShader(vertexShader);
            CheckShader(vertexShader, "TEXT VERTEX");

            // Compile fragment shader
            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, ShaderManager.TextFragmentShaderSrc);
            GL.CompileShader(fragmentShader);
            CheckShader(fragmentShader, "TEXT FRAGMENT");

            // Link shaders into a program
            _textShaderProgram = GL.CreateProgram();
            GL.AttachShader(_textShaderProgram, vertexShader);
            GL.AttachShader(_textShaderProgram, fragmentShader);
            GL.LinkProgram(_textShaderProgram);
            CheckProgram(_textShaderProgram, "TEXT PROGRAM");

            // Delete shaders as they're linked now
            GL.DetachShader(_textShaderProgram, vertexShader);
            GL.DetachShader(_textShaderProgram, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }

        /// <summary>
        /// Creates and compiles the shader program.
        /// </summary>
        private static int CreateShaderProgram(string vertexSrc, string fragmentSrc)
        {
            int vertex = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertex, vertexSrc);
            GL.CompileShader(vertex);
            CheckShader(vertex, "VERTEX");

            int fragment = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragment, fragmentSrc);
            GL.CompileShader(fragment);
            CheckShader(fragment, "FRAGMENT");

            int program = GL.CreateProgram();
            GL.AttachShader(program, vertex);
            GL.AttachShader(program, fragment);
            GL.LinkProgram(program);
            CheckProgram(program, "PROGRAM");

            GL.DetachShader(program, vertex);
            GL.DetachShader(program, fragment);
            GL.DeleteShader(vertex);
            GL.DeleteShader(fragment);

            return program;
        }

        /// <summary>
        /// Checks shader compilation status.
        /// </summary>
        private static void CheckShader(int handle, string type)
        {
            GL.GetShader(handle, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string info = GL.GetShaderInfoLog(handle);
                Console.WriteLine($"ERROR::{type}::Compile\n{info}");
            }
        }

        /// <summary>
        /// Checks program linking status.
        /// </summary>
        private static void CheckProgram(int program, string type)
        {
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0)
            {
                string info = GL.GetProgramInfoLog(program);
                Console.WriteLine($"ERROR::{type}::Link\n{info}");
            }
        }

        internal static void Cleanup()
        {
            // Cleanup Text
            GL.DeleteBuffer(_textVbo);
            GL.DeleteVertexArray(_textVao);
            GL.DeleteBuffer(_textEbo);
            GL.DeleteProgram(_textShaderProgram);

            // Cleanup shaders
            GL.DeleteProgram(_terrainShaderProgram);
            GL.DeleteProgram(_gridShaderProgram);
        }
    }
}