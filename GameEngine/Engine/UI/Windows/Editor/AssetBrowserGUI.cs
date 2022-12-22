using System.Numerics;
using GameEngine.Engine.Core;
using GameEngine.Engine.Utilities;
using ImGuiNET;


namespace GameEngine.Engine.UI.Windows.Editor;

public static class AssetBrowserGUI
{
    private static DirectoryInfo? _currentDirectory = null;
    
    public static void OnGuI()
    {
        if (_currentDirectory == null)
        {
            _currentDirectory = new DirectoryInfo(Window.GetInstance().CurrentProjectDirectory + @"\Assets\");
        }
        
        ImGui.Begin("Asset Browser ", ImGuiWindowFlags.MenuBar);
        ImGui.BeginMenuBar();
        ImGui.Text(_currentDirectory.FullName.Remove(0,Window.GetInstance().CurrentProjectDirectory.Length));
        
        if (_currentDirectory.Name != "Assets")
        {
            if (ImGui.Button("Back"))
            {
                _currentDirectory = _currentDirectory.Parent;
            }
            ImGui.SameLine();
        }
        
        ImGui.EndMenuBar();
        
        float padding = 16.0f;
        float thumbnailSize = 128.0f;
        float cellSize = thumbnailSize + padding;

        float panelWidth = ImGui.GetContentRegionAvail().X;
        int columnCount = (int)(panelWidth / cellSize);
        if (columnCount < 1)
            columnCount = 1;

        

        
        ImGui.Columns(columnCount, "0", false);
        foreach (var directory in _currentDirectory.GetDirectories())
        {
            ImGui.PushID(directory.Name);
            
            if (ImGui.ImageButton((IntPtr)AssetPool.GetTexture(@"Resources/Images/Icons/DirectoryIcon.png", false).TexId, new Vector2(128,128)))
            {
                _currentDirectory = directory;
            }
            ImGui.Text(directory.Name);
            ImGui.TextWrapped(_currentDirectory.Name);
            ImGui.NextColumn();
            
            ImGui.PopID();
        }
        
        ImGui.Columns(1);
        
        foreach (var file in _currentDirectory.GetFiles())
        {
            ImGui.PushID(file.Name);
            ImGui.Text(file.Name);
            ImGui.PopID();
        }
        
        ImGui.End();
    }

    
}