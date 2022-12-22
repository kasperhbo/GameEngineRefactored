using System.Numerics;
using ImGuiNET;

namespace GameEngine.Engine.Components;

public class BoxCollider2D : Component
{
    public Vector2 Offset = new Vector2();
    public Vector2 Size = new Vector2(0.5f, 0.5f);

    public float Density = 1.0f;
    public float Friction = 0.5f; 
    public float Restitution = 0.0f; 
    public float RestitutionThreshold = 0.5f; 
    
    public BoxCollider2D()
    {
        Type = this.GetType().Name;
    }
    public override void Update(float dt)
    {
    }

    public override void Render()
    {
    }

    public override void OnGui()
    {
        ImGui.SliderFloat2("Offset: ", ref Offset                                       ,0, 10);
        ImGui.SliderFloat2("Size: ", ref Size                                           ,0, 10);
                                                                                        
        ImGui.SliderFloat("Density: ",                ref Density                      ,0, 10);
        ImGui.SliderFloat("Friction: ",               ref Friction                     ,0, 10);
        ImGui.SliderFloat("Restitution: ",            ref Restitution                  ,0, 10);
        ImGui.SliderFloat("RestitutionThreshold: ",   ref RestitutionThreshold         ,0, 10);
    }

    public override void Destroy()
    {
    }

    public override void SetComponentType()
    {
    }
}