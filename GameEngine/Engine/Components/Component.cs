using GameEngine.Engine.Core;
using GameEngine.Engine.Serializing;
using Newtonsoft.Json;

namespace GameEngine.Engine.Components;

[JsonConverter(typeof(ComponentSerializer))]
public abstract class Component
{
    public string Type {get; protected set;}= "Component";

    [JsonIgnore]public Gameobject Parent  {get; private set; } = null;
    [JsonIgnore]public bool initialized   {get; private set; } = false;
    [JsonIgnore]public bool started       {get; private set; } = false;

    public virtual void Init(Gameobject parent)
    {
        initialized = true;
        this.Parent = parent;
    }

    public virtual void Start()
    {
        if(started) return;
        started = true;
    }

    public abstract void Update(float dt);
    public abstract void Render();
    public abstract void OnGui();
    public abstract void Destroy();
    public abstract void SetComponentType();
}
 