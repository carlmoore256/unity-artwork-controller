
public interface IObserver
{
    void OnChanged();
}

public interface IObservable
{
    void RegisterObserver(IObserver observer);
    void UnregisterObserver(IObserver observer);
    void NotifyObservers();
}

public interface IObserver<T>
{
    void OnChanged(T value);
}

public interface IObservable<T>
{
    void RegisterObserver(IObserver<T> observer);
    void UnregisterObserver(IObserver<T> observer);
    void NotifyObservers();
}
