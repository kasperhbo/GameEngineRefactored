namespace GameEngine.Engine.Scenes;

public class LevelEditorSceneInitializor : SceneInitializer
{
    public override void Init(Scene scene)
    {
        scene.SaveSceneOnDestroy = true;
    }

    public override void OnGui()
    {
        
    }
}