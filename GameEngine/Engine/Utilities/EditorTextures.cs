using GameEngine.Engine.Rendering;

namespace GameEngine.Engine.Utilities;

public static class EditorTextures
{
    public static Texture PlayIcon { get; private set; }     = new Texture("Resources/Images/Icons/Play.png", false);
    public static Texture PauseIcon { get; private set; }    = new Texture("Resources/Images/Icons/Pause.png", false);
    public static Texture SimulateIcon { get; private set; } = new Texture("Resources/Images/Icons/Simulate.png", false);
}
