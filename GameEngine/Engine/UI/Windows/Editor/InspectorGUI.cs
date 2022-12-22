using GameEngine.Engine.Core;
using ImGuiNET;

namespace GameEngine.Engine.UI.Windows.Editor;

public static class InspectorGUI
{
    public static void OnGui(Asset? currentAsset)
    {
        if (currentAsset != null)
        {
            ImGui.Begin("Inspector");
            
            currentAsset.OnGui();
            
            ImGui.End();
        }
    }
}