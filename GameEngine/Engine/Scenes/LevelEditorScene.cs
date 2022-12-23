using System.Numerics;
using GameEngine.Engine.Components;
using GameEngine.Engine.Core;
using GameEngine.Engine.Inputs;
using GameEngine.Engine.Rendering;
using GameEngine.Engine.UI.Windows.Editor;
using GameEngine.Engine.Utilities;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Window = GameEngine.Engine.Core.Window;

namespace GameEngine.Engine.Scenes;

public class LevelEditorScene : Scene
{
    public override void Init()
    {
        FrameBuffer = new FrameBuffer(Window.GetInstance().ClientSize.X, Window.GetInstance().ClientSize.Y);
        
        if(!LoadedScene)
        {
            Console.WriteLine("Not loaded the scene");
            Gameobject? ob = new Gameobject("Kasper", null,null);
            
            Sprite sp = new Sprite();
            sp.Init(AssetPool.GetTexture("Resources/Images/notexture.png", true));
            
            SpriteRenderer spr = new SpriteRenderer(sp);
            
            ob.AddComponent(spr);
            
            BoxCollider2D collider = (BoxCollider2D)ob.AddComponent(new BoxCollider2D());
            RigidBody rigid = (RigidBody)ob.AddComponent(new RigidBody());
            
            AddGameObjectToScene(ob);
        }
        
        _currentSelectedAsset = Gameobjects[0];
        Gameobjects[0].Transform.Rotation.Z = 32;
        
        base.Init();
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

        if (Input.KeyDown(Keys.Left))
        {
            CurrentCamera.position.X -= 100 * dt;
        }
        
        if (Input.KeyDown(Keys.Right))
        {
            CurrentCamera.position.X += 100 * dt;
        }
        
        if (Input.KeyDown(Keys.Up))
        {
            CurrentCamera.position.Y+= 100 * dt;
        }
        
        if (Input.KeyDown(Keys.Down))
        {
            CurrentCamera.position.Y -= 100 * dt;
        }
        
        if (Input.KeyPress(Keys.A))
            _enabled = !_enabled;

        if (_enabled)
            Gameobjects[0].Transform.Position = Input.MousePosition;

        if (Input.KeyDown(Keys.S))
        {
            SaveScene();
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
        base.PostRender();
        FrameBuffer.UnBind();
    }


    public LevelEditorScene(SceneInitializer sceneInitializer) : base(sceneInitializer)
    {
    }
}