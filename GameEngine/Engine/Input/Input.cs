using Accord.Math;
using GameEngine.Engine.Core;
using GameEngine.Engine.UI.Windows.Editor;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Matrix4x4 = System.Numerics.Matrix4x4;
using Vector2 = System.Numerics.Vector2;
using Vector4 = OpenTK.Mathematics.Vector4;
using Window = OpenTK.Windowing.GraphicsLibraryFramework.Window;

namespace GameEngine.Engine.Inputs;

public class Input
{
    private static List<Keys> keysDown;
    private static List<Keys> keysDownLast;
    private static List<MouseButton> buttonsDown;
    private static List<MouseButton> buttonsDownLast;
    private static GameWindow glfwWindow;
        
    public static Vector2 MousePosition { get; private set; }
    public static float ScrollX {get; private set;}
    public static float ScrollY {get; private set;}
    
    public static Vector2 GameViewportPos = new();
    public static Vector2 GameViewportSize = new();

    // public static float OrthoX()
    // {
    //     float currentX = MousePosition.X;
    //     
    //     currentX = (currentX / (float)glfwWindow.Size.X) * 2f - 1f;
    //     
    //     OpenTK.Mathematics.Vector4 tmp = new OpenTK.Mathematics.Vector4(currentX, 0, 0, 1);
    //     
    //     Camera camera = Core.Window.GetInstance().CurrentScene.CurrentCamera;
    //     
    //     Matrix4 view = camera.GetInverseView();
    //     Matrix4 proj = camera.GetInverseProjection();
    //
    //     Matrix4 viewProjection = Matrix4.Mult(proj, view);
    //
    //     tmp = Temp(viewProjection, tmp);
    //
    //     currentX = tmp.X;
    //         
    //     return currentX;
    // }
    //
    // public static float OrthoY()
    // {
    //     float currentY = Core.Window.GetInstance().Size.Y - MousePosition.Y;
    //     
    //     currentY = (currentY / Core.Window.GetInstance().Size.Y * 2f) - 1f;
    //     
    //     OpenTK.Mathematics.Vector4 tmp = new OpenTK.Mathematics.Vector4(0, currentY, 0, 1);
    //     
    //     Camera camera = Core.Window.GetInstance().CurrentScene.CurrentCamera;
    //     
    //     Matrix4 view = camera.GetInverseView();
    //     Matrix4 proj = camera.GetInverseProjection();
    //
    //     Matrix4 viewProjection = Matrix4.Mult(proj, view);
    //
    //     tmp = Temp(viewProjection, tmp);
    //     
    //     currentY = tmp.Y;
    //     
    //     return currentY;
    // }
    //
    public static float GetViewportMouseX()
    {
        float currentX = MousePosition.X - GameViewportPos.X;
        
        currentX = (currentX / (float)GameViewportSize.X) * 2f - 1f;
        
        Vector4 tmp = new Vector4(currentX, 0, 0, 1);

        Camera camera = Core.Window.GetInstance().CurrentScene.CurrentCamera;
        
        Matrix4 view = camera.GetInverseView();
        Matrix4 proj = camera.GetInverseProjection();
        
        Matrix4 viewProjection = Matrix4.Mult(proj, view);

        tmp = Temp(viewProjection, tmp);
        
        // tmp = (tmp * Core.Window.GetInstance().CurrentScene.CurrentCamera.GetInverseProjection())
        //       * Core.Window.GetInstance().CurrentScene.CurrentCamera.GetInverseView();
        
        currentX = tmp.X;
            
        return currentX;
    }

    public static float MouseX => GetWorld().X;
    public static float MouseY => GetWorld().Y;

    public static Vector2 GetWorld()
    {
        float currentX = MousePosition.X - GameViewportPos.X;
        currentX = (2.0f * (currentX / GameViewportSize.X)) - 1f;
        
        float currentY = MousePosition.Y - GameViewportPos.Y;
        currentY = -((2.0f * (1f - (currentY / GameViewportSize.Y))) - 1f);

        Camera camera = Core.Window.GetInstance().CurrentScene.CurrentCamera;
        Vector4 temp = new Vector4(currentX, currentY, 0, 1f);

        Matrix4 inverseView = camera.GetInverseView();
        Matrix4 inverseProj = camera.GetInverseProjection();
        
        Matrix4 viewProjection = Matrix4.Mult(inverseView, inverseProj);

        Vector4 outV = Temp(viewProjection, temp);
        return new Vector2(outV.X, outV.Y);
    }
    
    // public static float GetViewportMouseY()
    // {
    //     // float currentY = GameViewportPos.Y - MousePosition.Y;
    //     //
    //     // currentY = ((currentY / GameViewportSize.Y) * 2f - 1f);
    //     //
    //     // OpenTK.Mathematics.Vector4 tmp = new OpenTK.Mathematics.Vector4(0, currentY, 0, 1);
    //     //
    //     // // tmp = (tmp * Core.Window.GetInstance().CurrentScene.CurrentCamera.GetInverseProjection())
    //     // //       * Core.Window.GetInstance().CurrentScene.CurrentCamera.GetInverseView();
    //     // //       
    //     //
    //     // Camera camera = Core.Window.GetInstance().CurrentScene.CurrentCamera;
    //     //
    //     // Matrix4 view = camera.GetInverseView();
    //     // Matrix4 proj = camera.GetInverseProjection();
    //     //
    //     // Matrix4 viewProjection = Matrix4.Mult(proj, view);
    //     //
    //     // tmp = Temp(viewProjection, tmp);
    //     //
    //     // currentY = tmp.Y;
    //     //     
    //     float currentY 
    //     return currentY;
    // }
    //
    public static Vector4 Temp(Matrix4 matrix4, Vector4 vector4)
    {
        double xN;
        double yN;
        double zN;
        
        float x = vector4.X;
        float y = vector4.Y;
        float z = vector4.Z;
        float w = vector4.W;
        
        xN = Math.FusedMultiplyAdd(matrix4.M11, x, 
            Math.FusedMultiplyAdd(matrix4.M21, y, 
                Math.FusedMultiplyAdd(matrix4.M31, z, 
                    matrix4.M41 * w)));
        
        yN = Math.FusedMultiplyAdd(matrix4.M12, x,
            Math.FusedMultiplyAdd(matrix4.M22, y, 
                Math.FusedMultiplyAdd(matrix4.M32, z, 
                    matrix4.M42 * w)));
        
        zN = Math.FusedMultiplyAdd(matrix4.M13, x,
            Math.FusedMultiplyAdd(matrix4.M23, y, 
                Math.FusedMultiplyAdd(matrix4.M33, z, 
                    matrix4.M43 * w)));
      
        
        Vector4 dest = new((float)xN, (float)yN, (float)zN, w);
        
        return dest;
    }
    
    public static Vector4 Temp2(Matrix4 matrix4, Vector4 vector4)
    {
        double xN;
        double yN;
        double zN;
        
        float x = vector4.X;
        float y = vector4.Y;
        float z = vector4.Z;
        float w = vector4.W;
        
        xN = Math.FusedMultiplyAdd(   matrix4.M11, x, 
            Math.FusedMultiplyAdd(    matrix4.M21, y, 
                Math.FusedMultiplyAdd(matrix4.M31, z, 
                                      matrix4.M41 * w)));
        
        yN = Math.FusedMultiplyAdd(      matrix4.M12, x,
            Math.FusedMultiplyAdd(       matrix4.M22, y, 
                Math.FusedMultiplyAdd(   matrix4.M32, z, 
                                         matrix4.M42 * w)));
        
        zN = Math.FusedMultiplyAdd(      matrix4.M13, x,
            Math.FusedMultiplyAdd(       matrix4.M23, y, 
                Math.FusedMultiplyAdd(   matrix4.M33, z, 
                                         matrix4.M43 * w)));

        Vector4 dest = new((float)xN, (float)yN, (float)zN, w);
        
        return dest;
    }
    
    public static Vector4 Multiply(Matrix4 matrix, Vector4 vector) => matrix * vector;
    
    public static Vector4 Temp(Matrix4x4 matrix4, Vector4 vector4)
    {
        double xN;
        double yN;
        double zN;
        
        float x = vector4.X;
        float y = vector4.Y;
        float z = vector4.Z;
        float w = vector4.W;
        
        xN = Math.FusedMultiplyAdd(matrix4.M11, x, 
            Math.FusedMultiplyAdd(matrix4.M21, y, 
                Math.FusedMultiplyAdd(matrix4.M31, z, 
                    matrix4.M41 * w)));
        
        yN = Math.FusedMultiplyAdd(matrix4.M12, x,
            Math.FusedMultiplyAdd(matrix4.M22, y, 
                Math.FusedMultiplyAdd(matrix4.M32, z, 
                    matrix4.M42 * w)));
        
        zN = Math.FusedMultiplyAdd(matrix4.M13, x,
            Math.FusedMultiplyAdd(matrix4.M23, y, 
                Math.FusedMultiplyAdd(matrix4.M33, z, 
                    matrix4.M43 * w)));
      
        
        Vector4 dest = new((float)xN, (float)yN, (float)zN, w);
        
        return dest;
    }
    //
    public static void Initialize(GameWindow game)
    {
        keysDown = new List<Keys>();
        keysDownLast = new List<Keys>();
        buttonsDown = new List<MouseButton>();
        buttonsDownLast = new List<MouseButton>();

        CurrentKeyDown = -2;
        
        glfwWindow = game;

        game.MouseDown += game_MouseDown;
        game.MouseUp += game_MouseUp;
        game.KeyDown += game_KeyDown;
        game.KeyUp += game_KeyUp;

        game.MouseMove += args =>
        {
            MousePosition = new Vector2(args.X, args.Y);
        };

        game.MouseWheel += args =>
        {
            ScrollX = args.OffsetX;
            ScrollY = args.OffsetY;
        };

    }

    public static int CurrentKeyDown { get; private set; }

    static void game_KeyDown(KeyboardKeyEventArgs e)
    {
        CurrentKeyDown = (int)e.Key;
        if (!keysDown.Contains(e.Key))
            keysDown.Add(e.Key);
    }
    static void game_KeyUp(KeyboardKeyEventArgs e)
    {
        CurrentKeyDown = -2;
        while(keysDown.Contains(e.Key))
            keysDown.Remove(e.Key);
    }
    static void game_MouseDown(MouseButtonEventArgs e)
    {
        if (!buttonsDown.Contains(e.Button))
            buttonsDown.Add(e.Button);
    }
    static void game_MouseUp(MouseButtonEventArgs e)
    {
        while (buttonsDown.Contains(e.Button))
            buttonsDown.Remove(e.Button);
    }
    public static void Update()
    {
        keysDownLast = new List<Keys>(keysDown);
        buttonsDownLast = new List<MouseButton>(buttonsDown);
    }

    public static bool KeyPress(Keys key)
    {
        return (keysDown.Contains(key) && !keysDownLast.Contains(key));
    }
    public static bool KeyRelease(Keys key)
    {
        return (!keysDown.Contains(key) && keysDownLast.Contains(key));
    }
    public static bool KeyDown(Keys key)
    {
        return (keysDown.Contains(key));
    }

    public static bool MousePress(MouseButton button)
    {
        return (buttonsDown.Contains(button) && !buttonsDownLast.Contains(button));
    }
    public static bool MouseRelease(MouseButton button)
    {
        return (!buttonsDown.Contains(button) && buttonsDownLast.Contains(button));
    }
    public static bool MouseDown(MouseButton button)
    {
        return (buttonsDown.Contains(button));
    }
}