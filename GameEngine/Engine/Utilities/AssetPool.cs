using GameEngine.Engine.Rendering;
using OpenTK.Graphics.OpenGL;

namespace GameEngine.Engine.Utilities;

public class ShaderSource
{
    public string VertPath { get; private set; }
    public string FragPath { get; private set; }
    
    public ShaderSource(string defaultVert, string defaultFrag)
    {
        VertPath = defaultVert;
        FragPath = defaultFrag;
    }
}

public static class AssetPool
{
    private static Dictionary<string, Texture> Textures { get; set; } = new();
    private static Dictionary<ShaderSource, Shader> Shaders { get; set; } = new();
    
    public static Texture GetTexture(string path, bool flipped)
    {
        Texture texture = null;
        
        if (Textures.TryGetValue(path, out texture))
        {
            return texture;
        }

        texture = new Texture();
        texture.Init(path, flipped);
        
        Textures.Add(path, texture);
        
        return texture;
    }

    public static Shader GetShader(ShaderSource shaderSource)
    {
        if (Shaders.TryGetValue(shaderSource, out var shaderOut)) return shaderOut;

        var shader = new Shader(shaderSource.VertPath, shaderSource.FragPath);
        shader.Compile();
        Shaders.Add(shaderSource, shader);
        return shader;
    }
}