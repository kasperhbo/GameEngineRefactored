using GameEngine.Engine.Utilities;
using Newtonsoft.Json;
using OpenTK.Mathematics;
using Vector2 = System.Numerics.Vector2;

namespace GameEngine.Engine.Rendering;

public class Sprite
{
    public bool hasTexture = false;
    public string texturePath = "";
    
    
    [JsonIgnore]public Texture texture { get; private set; }
    
    public Vector2[] TexCoords = new[]
    {
        new Vector2(1, 1),
        new Vector2(1, 0),
        new Vector2(0, 0),
        new Vector2(0, 1)
    };

    public Sprite(bool hasTexture = false, string texturePath = "")
    {
        if (hasTexture)
        {
            Init(AssetPool.GetTexture(texturePath, false));
        }
    }
    
    public void Init(Texture texture)
    {
        hasTexture = true;
        texturePath = texture.Filepath;
        this.texture = texture;
    }
}