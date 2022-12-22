namespace GameEngine.Engine.Core;

public class EventSystem
{
    private static List<IObserver> _observers = new();
    
    public static void AddObserver(IObserver observer) {
        _observers.Add(observer);
    }
    
    public static void Notify(Gameobject obj, Event _event)
    {
        foreach (var observer in _observers)
        {
            observer.OnNotify(obj, _event);
        }
    }
}