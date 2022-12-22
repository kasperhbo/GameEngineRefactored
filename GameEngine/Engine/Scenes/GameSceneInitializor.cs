namespace GameEngine.Engine.Scenes;

public class GameSceneInitializor : SceneInitializer
{
    public override void Init(Scene scene)
    {
        scene.SaveSceneOnDestroy = false;
    }

    public override void OnGui()
    {
        
    }
}