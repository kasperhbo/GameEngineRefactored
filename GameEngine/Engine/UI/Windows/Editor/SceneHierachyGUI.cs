using GameEngine.Engine.Core;
using ImGuiNET;

namespace GameEngine.Engine.UI.Windows.Editor;

public static class SceneHierachyGUI
{
    public static void OnGui()
    {
        ImGui.Begin("Hierachy");
        
        List<Gameobject> gameobjects = Window.GetInstance().CurrentScene.Gameobjects;
        
        for (int i = 0; i < gameobjects.Count; i++)
        {
            ImGui.PushID(i);
            bool treeOpen = ImGuiNET.ImGui.TreeNodeEx(
                gameobjects[i].Name,
                    
                ImGuiTreeNodeFlags.DefaultOpen| 
                ImGuiTreeNodeFlags.FramePadding|
                ImGuiTreeNodeFlags.OpenOnArrow | 
                ImGuiTreeNodeFlags.SpanAvailWidth,
                    
                gameobjects[i].Name
            );

            ImGuiNET.ImGui.PopID();
            if (treeOpen)
            {
                ImGuiNET.ImGui.TreePop();
            }
        }

        ImGui.End();
    }
}