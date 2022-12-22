using System.Drawing;
using ImGuiNET;
using System.Numerics;
using System.Runtime.InteropServices;
using GameEngine.Engine.Core;
using GameEngine.Engine.Inputs;
using GameEngine.Engine.Scenes;
using GameEngine.Engine.Utilities;

namespace GameEngine.Engine.UI.Windows.Editor;

public static class GameViewport
{
    private static bool isPlaying = false;
    private static float leftX, rightX, topY, bottomY;
    public static bool IsInViewport { get; private set; } = false;
    
    public static void OnGui()
    {
        ImGui.Begin("Game Viewport",
                 ImGuiWindowFlags.NoScrollbar     
                 | ImGuiWindowFlags.NoScrollWithMouse 
                 | ImGuiWindowFlags.MenuBar       );
        
        ImGui.SetCursorPos(new Vector2(ImGui.GetCursorPosX(), ImGui.GetCursorPosY()));
        
        Vector2 windowSize = GetLargestSizeForViewport();
        Vector2 windowPos = GetCenteredPosForViewport(windowSize);
        
        ImGui.SetCursorPos(windowPos);

        int textureId = Window.GetInstance().CurrentScene.FrameBuffer.TextureID;
        ImGui.ImageButton((IntPtr)textureId, new Vector2(windowSize.X, windowSize.Y));

        if (ImGui.IsItemHovered())
        {
            IsInViewport = true;
        }
        else
        {
            IsInViewport = false;
        }
        
        Input.GameViewportPos =  windowPos with  { X = (windowPos.X) , Y = windowPos.Y};
        Input.GameViewportSize = windowPos with  { X = (windowSize.X), Y = windowSize.Y};
        
        Vector2 vMin = ImGui.GetWindowContentRegionMin();
        Vector2 vMax = ImGui.GetWindowContentRegionMax();

        vMin.X += ImGui.GetWindowPos().X;
        vMin.Y += ImGui.GetWindowPos().Y;
        vMax.X += ImGui.GetWindowPos().X;
        vMax.Y += ImGui.GetWindowPos().Y;
        
        Toolbar(vMin.X , vMax.X - vMin.X, vMin.Y, vMax.Y - vMin.Y);
        
        ImGui.End();
    }

    /// <summary>
    /// Stolen from the hazel engine xxx
    /// </summary>
    /// <param name="viewportXPos"></param>
    /// <param name="viewportBoundsX"></param>
    /// <param name="viewportYPos"></param>
    /// <param name="viewportBoundsY"></param>
    private static void Toolbar(float viewportXPos, float viewportBoundsX, float viewportYPos, float viewportBoundsY)
    {
        bool _false = false;
        
        float buttonSize = 12.0f + 5.0f;
        float edgeOffset = 4.0f;
        float windowHeight = 32.0f; // annoying limitation of ImGui, window can't be smaller than 32 pixels
        
        float numberOfButtons = 2.0f;

        float backgroundWidth = (edgeOffset * 6.0f + buttonSize * numberOfButtons +
                                 edgeOffset * (numberOfButtons - 1.0f) * 2.0f) + 20;

        float toolbarX = (viewportXPos + viewportBoundsX) / 2.0f;

        ImGui.SetNextWindowPos(new Vector2(toolbarX - (backgroundWidth / 2.0f), viewportYPos + edgeOffset));
        ImGui.SetNextWindowSize(new Vector2(backgroundWidth, windowHeight + 10));
        
        ImGui.SetNextWindowBgAlpha(0.0f);
        
        ImGui.Begin("##toolbar", ref _false, ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoDocking);
        
        float desiredHeight = 26.0f + 5.0f;
        
        if(!isPlaying)
        {
            if (ImGui.ImageButton((IntPtr)EditorTextures.PlayIcon.TexId, new Vector2(buttonSize, buttonSize)))
            {
                EventSystem.Notify(null, new Event(EventType.GameEngineStartPlay));
                isPlaying = true;
            }
        }
        else
        {
            if (ImGui.ImageButton((IntPtr)EditorTextures.PauseIcon.TexId, new Vector2(buttonSize, buttonSize)))
            {
                EventSystem.Notify(null, new Event(EventType.GameEngineStopPlay));
                isPlaying = false;
            }
        }
        
        ImGui.SameLine();
        
        if (!isPlaying)
            ImGui.ImageButton((IntPtr)EditorTextures.SimulateIcon.TexId, new Vector2(buttonSize, buttonSize));
        else
        {
            //TODO: REPLACE WITH STOP ICON!
            if (ImGui.ImageButton((IntPtr)EditorTextures.PauseIcon.TexId, new Vector2(buttonSize, buttonSize)))
            {
                isPlaying = false;
            }
        }
        
        
        ImGui.End();
    }
    
    private static Vector2 GetLargestSizeForViewport()
    {
        Vector2 windowSize = ImGui.GetContentRegionAvail();

        float aspectWidth = windowSize.X;
        float aspectHeight = aspectWidth / Window.GetInstance().TargetAspectRatio;
        
        if (aspectHeight > windowSize.Y)
        {
            // We must switch to pillarbox mode
            aspectHeight = windowSize.Y;
            aspectWidth = aspectHeight * Window.GetInstance().TargetAspectRatio;
        }

        return new Vector2(aspectWidth, aspectHeight);
    }
        
    private static Vector2 GetCenteredPosForViewport(Vector2 aspectSize)
    {
        Vector2 windowSize = new Vector2();
        
        windowSize = ImGui.GetContentRegionAvail();

        float viewPortX = (windowSize.X / 2.0F) - (aspectSize.X / 2.0F);
        float viewPortY = (windowSize.Y / 2.0F) - (aspectSize.Y / 2.0F);

        return new Vector2(viewPortX + ImGui.GetCursorPosX(), viewPortY+ ImGui.GetCursorPosY());
    }
    
    private static Rectangle RectangleTest(int x, int y)
    {
        Rectangle resulRectangle = new();
        resulRectangle.X -= x;
        resulRectangle.Y -= y;
        resulRectangle.Width += x;
        resulRectangle.Height += y;
        return resulRectangle;
    }
}