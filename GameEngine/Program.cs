using GameEngine.Engine.Core;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace GameEngine;

internal class Program{
    
    static void Main(string[] args)
    {
        Console.WriteLine("Hello Kasper's Game Engine!");
        GameWindowSettings gameWindowSettings = new GameWindowSettings();
        
        NativeWindowSettings nativeWindowSettings = new NativeWindowSettings();
        
        nativeWindowSettings.Title = "Kasper's Game Engine";
        nativeWindowSettings.Size = new Vector2i(1920, 1080);
        
        Window.GetInstance(gameWindowSettings, nativeWindowSettings, @"D:\GameEngineProjects\Project01").Run();
        
        
    }
}