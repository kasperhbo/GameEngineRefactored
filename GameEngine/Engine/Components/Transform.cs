using System.Numerics;

namespace GameEngine.Engine.Components;

public class Transform
{
    public Vector2 Position;
    public Vector2 Scale;
    public Vector3 Rotation = new Vector3();
    
    public Transform()
    {
        Position = Vector2.Zero;
        Rotation = Vector3.Zero;
        Scale = Vector2.Zero;
        
    }

    public Transform(Vector2 position,Vector3 rotation, Vector2 scale)
    {
        Position = position;
        Rotation = rotation;
        Scale = scale;
    }


    public Transform Copy()
    {
        var t = new Transform(new Vector2(Position.X, Position.Y), new Vector3(Rotation.X, Rotation.Y, Rotation.Z), new Vector2(Scale.X, Scale.Y));
        return t;
    }
    
    public void Copy(Transform to)
    {
        to.Position = Position;
        to.Rotation = Rotation;
        to.Scale = Scale;
    }

    public override bool Equals(object o)
    {
        if (o == null) return false;
        if (!(o is Transform)) return false;

        var t = (Transform)o;
        return t.Position.Equals(Position) && t.Scale.Equals(Scale) && t.Rotation.Equals(Rotation);
    }
    
}