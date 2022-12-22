using System.Numerics;
using GameEngine.Engine.Components;
using GameEngine.Engine.UI;
using ImGuiNET;
using Newtonsoft.Json;

namespace GameEngine.Engine.Core;

public class Gameobject : Asset
{
    public string Name { get; set; }
    public Transform Transform { get; set; }

    [JsonProperty]public List<Component>? Components { get; private set; } = new();

    public string Type = "";

    public Gameobject(string name, Transform? transform, List<Component>? components)
    {
        this.Type = GetType().Name;
        this.Name = name;
        
        if (transform == null)
        {
            this.Transform = new Transform();
            
            Transform.Position = new System.Numerics.Vector2(0,0);
            Transform.Scale = new System.Numerics.Vector2(32,32);
        }else{
            this.Transform = transform;
        }
 
        if(components == null)
        {
            this.Components = new List<Component>();       
        }else{
            this.Components = components;
        }
    }
    
    public void Init()
    {
        foreach (var component in Components)
        {
            if(component != null)
                component.Init(this);
        }
    }

    public void Start()
    {   
        foreach (var component in Components)
        {
            if(component != null)
                component.Start();
        }
    }
    
    public void Update(float dt)
    {
        foreach (var component in Components)
        {
            if(component != null)
                component.Update(dt);
        }
    }
    
    public void Render()
    {
        foreach (var component in Components)
        {
            if(component != null)
                component.Render();
        }
    }
    
    public void OnGui()
    {
        ImGui.SliderFloat2("Position: ", ref Transform.Position, 0, 10);
        ImGui.SliderFloat3("Rotation: ", ref Transform.Rotation, 0, 10);
        ImGui.SliderFloat2("Scale: ", ref Transform.Scale, 0, 32);
                
        foreach (var component in Components)
        {
            if(component != null)
                component.OnGui();
        }
        
    }
    
    public void Destroy()
    {
        foreach (var component in Components)
        {
            if(component != null)
                component.Destroy();
        }
    }
    
    
    public virtual T GetComponent<T>() where T : Component
    {
        foreach (var component in Components)
        {
            if (typeof(T) == component.GetType()) return (component as T)!;
        }

        return null;
    }
    
    public virtual Component RemoveComponent<T>(Type componentClass) where T : Component
    {
        for (var i = 0; i < Components.Count; i++)
        {
            var c = Components[i];
            
            if (componentClass.IsAssignableFrom(c.GetType()))
            {
                Components.RemoveAt(i);
                return default;
            }
        }

        return null;
    }

    public Component AddComponent(Component c)
    {
        Components.Add(c);
        return c;
    }


  
}