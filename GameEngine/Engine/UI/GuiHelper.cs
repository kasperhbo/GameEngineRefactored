using System.Numerics;
using ImGuiNET;
using GameEngine.Engine.Inputs;

namespace GameEngine.Engine.UI;

public class GuiHelper
{
    public static bool IsInScreen()
    {
        Vector2 windowSize = ImGui.GetContentRegionAvail();
        Vector2 topLeft = ImGuiNET.ImGui.GetCursorScreenPos();            
            
        topLeft.X -= ImGui.GetScrollX();
        topLeft.X -= ImGui.GetScrollY();
            
        float leftX = topLeft.X;
        float bottomY = topLeft.Y;
        float rightX = topLeft.X + windowSize.X;
        float topY = topLeft.Y + windowSize.Y;
            
        return Input.MousePosition.X >= leftX &&   Input.MousePosition.X <= rightX &&
               Input.MousePosition.Y >= bottomY && Input.MousePosition.Y <= topY;   
    }

    public static Vector2 DragFloat2(string label, Vector2 val)
    {
        Vector2 outval = val;
        
        ImGui.PushID(label);
        ImGui.Text(label);
        ImGui.SameLine();
        ImGui.DragFloat2("", ref outval);
        ImGui.PopID();
        
        return outval;
    }
}