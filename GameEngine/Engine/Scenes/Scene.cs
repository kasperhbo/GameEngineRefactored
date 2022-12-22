using System.Resources;
using Box2DSharp.Dynamics;
using GameEngine.Engine.Core;
using GameEngine.Engine.Rendering;
using Newtonsoft.Json;
using OpenTK.Mathematics;

namespace GameEngine.Engine.Scenes;

public class Scene
{
    public List<Gameobject?> Gameobjects { get; private set; } = new();
    public Camera? CurrentCamera { get; private set; }
    public Renderer? Renderer { get; private set; }
    public FrameBuffer FrameBuffer { get; protected set; }
    public bool SaveSceneOnDestroy { get; set; }

    private bool started = false;

    protected Asset _currentSelectedAsset;
    
    private SceneInitializer sceneInitializer;
    
    public Scene(SceneInitializer sceneInitializer)
    {
        this.sceneInitializer = sceneInitializer;
        Renderer = new Renderer();
        CurrentCamera = new Camera(new Vector3(0,0,0));
        sceneInitializer.Init(this);
    }
    
    public virtual void Init()
    {
        started = true;
        
        foreach (var gameobject in Gameobjects)
        {
            gameobject!.Init();
        }
    }

    public virtual void Start()
    {
        foreach (var gameobject in Gameobjects)
        {
            gameobject!.Start();
        }
    }

    public static bool IsRunning = false;

    public virtual void Update(float dt)
    {
        foreach (var gameobject in Gameobjects)
        {
            gameobject!.Update(dt);
        }
    }

    public virtual void PreRender()
    {
        
    }
    
    public virtual void Render()
    {
        Renderer.Render();
        
        foreach (var gameobject in Gameobjects)
        {
            gameobject!.Render();
        }
        
        PostRender();
    }

    public virtual void PostRender()
    {
        
    }
    
    protected void AddGameObjectToScene(Gameobject go)
    {
        if (go == null) throw new ArgumentNullException(nameof(go));
        
        //TODO: ADD POSSIBILITY TO ADD OBJECTS ON RUNTIME
        if (IsRunning) return;
        
        Gameobjects.Add(go);
        
        if(!started)
        {
            return;
        }
        
        go.Init();
        go.Start();
    }
    
    public virtual void OnGui()
    {
        
    }
    
    public virtual void Destroy()
    {
        //TODO: ADD POSSIBILITY TO DESTROY OBJECTS ON RUNTIME
        if (IsRunning) return;
        
        foreach (var gameobject in Gameobjects)
        {
            gameobject!.Destroy();
        }        
    }

    public void WindowResized(int width, int height)
    {
        FrameBuffer = new FrameBuffer(width, height);
    }

    protected bool LoadedScene = false;
    public void LoadScene()
    {
        string path = "Resources/testScene.json";
        if(File.Exists(path))
        {
            LoadedScene = true;
            List<Gameobject?> objs = JsonConvert.DeserializeObject<List<Gameobject>>(File.ReadAllText(path))!;

            foreach (var t in objs!)
            {
                AddGameObjectToScene(t!);
            }
        }
    }

    public void SaveScene()
    {
        if(SaveSceneOnDestroy)
        {
            var objects = Gameobjects.ToArray();

            string sceneData = JsonConvert.SerializeObject(objects, Formatting.Indented);
            string path = "Resources/testScene.json";

            Console.WriteLine("Saving scene at: " + path);

            if (File.Exists(path))
            {
                File.WriteAllText(path, sceneData);
            }
            else
            {
                using (FileStream fs = File.Create(path))
                {
                    fs.Close();
                }

                File.WriteAllText(path, sceneData);
            }
        }
    }
}