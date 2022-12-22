using Box2DSharp.Dynamics;
using GameEngine.Engine.Core;
using GameEngine.Engine.Scenes;

namespace GameEngine.Engine.Components;

public class RigidBody : Component
{
    public BodyType BodyType { get; set; }
    public bool FixedRotation = false;

    public Body runtimeBody = null;
    
    public RigidBody()
    {
        Type = this.GetType().Name;
        BodyType = BodyType.DynamicBody;
        
        //TODO: FIND SOMETHING BETTER FOR THIS
        Scene currentScene = Window.GetInstance().CurrentScene;
        if(currentScene.GetType() == typeof(GameScene))
        {
            GameScene scene = (GameScene)currentScene;
            scene.RigidBodies.Add(this);
        }
        

    }
    
    public override void Update(float dt)
    {
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