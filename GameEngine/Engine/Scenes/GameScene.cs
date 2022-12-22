using System.Numerics;
using Box2DSharp.Collision.Shapes;
using Box2DSharp.Dynamics;
using GameEngine.Engine.Components;
using GameEngine.Engine.Core;
using GameEngine.Engine.Inputs;
using GameEngine.Engine.Rendering;
using GameEngine.Engine.UI.Windows.Editor;
using GameEngine.Engine.Utilities;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Window = GameEngine.Engine.Core.Window;

namespace GameEngine.Engine.Scenes;

/// <summary>
/// Scene that gets created on play
/// </summary>
public class GameScene : Scene
{
    private World physicsWorld = null;
    public List<RigidBody> RigidBodies = new();
    
    public GameScene(SceneInitializer sceneInitializer) : base(sceneInitializer)
    {
    }
    
    public override void Init()
    {
        //Creating frame buffer to render game to   
        FrameBuffer = new FrameBuffer(Window.GetInstance().ClientSize.X, Window.GetInstance().ClientSize.Y);
        physicsWorld = new World(new Vector2(0, 9.8f));
 
        base.Init();
        
        //Creating Rigidbody's in the world
        foreach (var rb in RigidBodies)
        {
            Console.WriteLine("Adding rb to world");
            Gameobject go = rb.Parent;
            Transform transform = go.Transform;

            BodyDef bodyDef = new BodyDef();
            
            bodyDef.BodyType = rb.BodyType;
            bodyDef.Position = transform.Position;
            
            Body body = physicsWorld.CreateBody(bodyDef);
            body.IsFixedRotation = rb.FixedRotation;

            rb.runtimeBody = body;

            BoxCollider2D bo = go.GetComponent<BoxCollider2D>();
            if (bo != null)
            {
                PolygonShape shape = new PolygonShape();
                shape.SetAsBox(transform.Scale.X * bo.Size.X, transform.Scale.Y * bo.Size.Y);

                FixtureDef fixtureDef = new FixtureDef();
                fixtureDef.Shape = shape;
                fixtureDef.Density                  = bo.Density;
                fixtureDef.Friction = bo.Friction;
                fixtureDef.Restitution = bo.Restitution;
                fixtureDef.RestitutionThreshold = bo.RestitutionThreshold;
                
                body.CreateFixture(fixtureDef);
            }
        }
    }

    public override void OnGui()
    {
        GameViewport.OnGui();
        SceneHierachyGUI.OnGui();
        AssetBrowserGUI.OnGuI();
        InspectorGUI.OnGui(_currentSelectedAsset);
        
        base.OnGui();
    }

    private bool _enabled = false;
    public override void Update(float dt)
    {
        base.Update(dt);
        
        
        
        if (Input.KeyPress(Keys.A))
           _enabled = !_enabled;
        
        
        //Physics
        {
            int velocityItterations = 6;
            int positionItterations = 2;
            physicsWorld.Step(dt, velocityItterations, positionItterations);

            foreach (var rb in RigidBodies)
            {
                Gameobject go = rb.Parent;
                Transform transform = go.Transform;

                Body body = rb.runtimeBody;

                transform.Position = body.GetPosition();
                transform.Rotation.Z = body.GetAngle();
            }
        }
    }

    public override void PreRender()
    {
        base.PreRender();
        FrameBuffer.Bind();
    }
    
    public override void Render()
    {
        base.Render();
    }

    public override void PostRender()
    {
        FrameBuffer.UnBind();
        base.PostRender();
    }
}