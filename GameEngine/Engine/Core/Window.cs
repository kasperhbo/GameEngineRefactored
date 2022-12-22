using System.Xml.Schema;
using Engine;
using GameEngine.Engine.Inputs;
using GameEngine.Engine.Rendering;
using GameEngine.Engine.Scenes;
using ImGuiNET;
using OpenTK.Graphics.ES11;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace GameEngine.Engine.Core;

public class Window : GameWindow, IObserver
{
    private static Window? _instance = null;
    private ImGuiController? _imGuiController = null;

    public Scene CurrentScene { get; private set; }
    public string CurrentProjectDirectory { get; private set; }
    public float TargetAspectRatio => 16.0f / 9.0f;
    
    private Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings, string currentProjectDirectory) : base(gameWindowSettings, nativeWindowSettings)
    {
        EventSystem.AddObserver(this);
        this.CurrentProjectDirectory = currentProjectDirectory;
    }
    
    public static Window GetInstance(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings, string currentProjectDirectory)
    {
        _instance = new Window(gameWindowSettings, nativeWindowSettings, currentProjectDirectory);
        
        return _instance;
    }
    
    public static Window GetInstance()
    {
        if(_instance == null)
        {
            throw new Exception("Window has not been initialized yet!, Initialize it with GetInstance(GameWindowSettings, NativeWindowSettings)");
        }
        
        return _instance;
    }

    protected override void OnLoad()
    {
        Console.WriteLine("Load open TK");
        base.OnLoad();
        
        GL.Viewport(0,0,1920,1080);
        Input.Initialize(this);

        CurrentScene = new LevelEditorScene(new LevelEditorSceneInitializor());
        
        CurrentScene.LoadScene();
        CurrentScene.Init();
        CurrentScene.Start();
        
        //Create new gui controller
        
        _imGuiController = new ImGuiController(ClientSize.X, ClientSize.Y);
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);
        CurrentScene.Update((float)args.Time);
    }
    
    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);
        
        CurrentScene.PreRender();
        
        GL.ClearColor(.3f, .3f, .3f, 1);
        GL.Clear(ClearBufferMask.ColorBufferBit);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.One, BlendingFactor.OneMinusSrcAlpha);

        if (args.Time > 0)
        {
            CurrentScene.Render();
        }

        _imGuiController?.Update(this, (float)args.Time);
        ImGui.DockSpaceOverViewport();
        CurrentScene.OnGui();
        _imGuiController!.Render();
        ImGuiController.CheckGLError("End of frame");

        SwapBuffers();
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        
        CurrentScene.WindowResized(e.Width, e.Height);        
        _imGuiController!.WindowResized(e.Width,e.Height);
        
        GL.Viewport(0, 0, e.Width, e.Height);
        base.OnResize(e);
    }
    
    protected override void OnUnload()
    {
        Console.WriteLine("Unload Game Engine");
        if(CurrentScene.SaveSceneOnDestroy)
            CurrentScene.SaveScene();
        base.OnUnload();
    }

    public void OnNotify(Gameobject obj, Event _event)
    {
        if (_event.type == EventType.GameEngineStartPlay)
        {
            Console.WriteLine("Start Engine Runtime");
            List<Gameobject> _gameobjects = new List<Gameobject>();

            Gameobject[] go = CurrentScene.Gameobjects.ToArray();
                
            CurrentScene = new GameScene(new GameSceneInitializor());
            CurrentScene.LoadScene();
            CurrentScene.Init();
            CurrentScene.Start();
        } 
        
        if (_event.type == EventType.GameEngineStopPlay)
        {
            Console.WriteLine("Stop Engine Runtime");
            CurrentScene = new LevelEditorScene(new LevelEditorSceneInitializor());
            CurrentScene.LoadScene();
            CurrentScene.Init();
            CurrentScene.Start();
        } 
    }
}
