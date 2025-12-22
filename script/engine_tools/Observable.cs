using System;
using System.Collections.Generic;

namespace Pew;

public class Observable<T>
{
    private T _field;
    private readonly List<Action<T>> _observers = new List<Action<T>>();

    public Observable() { }

    public Observable(T initialValue)
    {
        _field = initialValue;
    }

    // Property to get/set the value
    public T Value
    {
        get => _field;
        set
        {
            if (!EqualityComparer<T>.Default.Equals(_field, value))
            {
                _field = value;
                NotifyObservers();
            }
        }
    }

    public void Subscribe(Action<T> observer)
    {
        if (observer != null && !_observers.Contains(observer))
            _observers.Add(observer);
    }

    public void Unsubscribe(Action<T> observer)
    {
        if (observer != null)
            _observers.Remove(observer);
    }

    private void NotifyObservers()
    {
        foreach (var observer in _observers)
            observer(_field);
    }

    public static implicit operator T(Observable<T> observable) => observable._field;
}