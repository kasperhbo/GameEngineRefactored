namespace GameEngine.Engine.Core;

public interface IObserver
{
    public void OnNotify(Gameobject obj, Event _event);
}