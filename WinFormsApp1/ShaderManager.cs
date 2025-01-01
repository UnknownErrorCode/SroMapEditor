// GridGame.cs
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
        public const string TerrainFragmentShaderSrc = @"
#version 330 core
in vec3 vNormal;
in vec3 vFragPos;
in vec2 vTexCoord; // Receive texture coordinates

out vec4 FragColor;

uniform sampler2D uTexture; // Texture sampler

void main()
{
    vec3 ambient = vec3(0.3) * texture(uTexture, vTexCoord).rgb; // Ambient with texture
    FragColor = vec4(vTexCoord, 0.0, 1.0); // Visualize texture coordinates
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
    }
}