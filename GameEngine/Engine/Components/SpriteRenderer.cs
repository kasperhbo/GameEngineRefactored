using GameEngine.Engine.Core;
using GameEngine.Engine.Rendering;
using Newtonsoft.Json;
using OpenTK.Mathematics;
using Vector4 = System.Numerics.Vector4;
using Vector2 = System.Numerics.Vector2;

namespace GameEngine.Engine.Components;

public class SpriteRenderer : Component
{
    public Vector4 Color { get; set; } = new Vector4(1, 1, 1, 1);
    
    private Transform lastTransform = new();
    public Sprite sprite;
    public int ZIndex = 0;
        
    [JsonIgnore]public bool IsDirty = true;
    [JsonIgnore]public Texture Texture => sprite.texture;
    [JsonIgnore]public Vector2[] TextureCoords => sprite.TexCoords;

    public SpriteRenderer(Sprite sprite, int zIndex = 0)
    {
        Type = this.GetType().Name;
        
        this.sprite = sprite;
        
        ZIndex = zIndex;
        
        Color = new Vector4(1,1,1,1);
    }
    
    public override void Start()
    {        
        Window.GetInstance().CurrentScene.Renderer?.AddSprite(this);
    }

    public override void Update(float dt)
    {
        // Console.WriteLine(this._sprite.texture);
        if (!lastTransform.Equals(Parent.Transform))
        {
            // Console.WriteLine("Transform changed");
            Parent.Transform.Copy(lastTransform);
            IsDirty = true;
        }

        this.IsDirty = true;
        // if(this.IsDirty && Parent.isNew)
        //     Console.WriteLine("is dirty");
    }

    public override void Render()
    {
        
    }

    public override void OnGui()
    {
        
    }

    public override void Destroy()
    {
        
    }

    public override void SetComponentType()
    {
    }
}