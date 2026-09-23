using System;

/// <summary>
/// Marker interface – każde zdarzenie musi go implementować.
/// Może być pustą strukturą (struct) lub klasą z danymi.
/// </summary>
public interface IEvent { }

/// <summary>
/// Interfejs bindingu – pozwala na własne implementacje (np. z priorytetem).
/// </summary>
public interface IEventBinding<TEvent> where TEvent : IEvent
{
    void Invoke(TEvent @event);
    bool MatchesHandler(Action<TEvent> handler);
}

/// <summary>
/// Domyślna implementacja bindingu – wrapper na zwykły Action.
/// </summary>
public class EventBinding<TEvent> : IEventBinding<TEvent> where TEvent : IEvent
{
    private readonly Action<TEvent> _handler;

    public EventBinding(Action<TEvent> handler)
    {
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
    }

    public void Invoke(TEvent @event) => _handler.Invoke(@event);

    public bool MatchesHandler(Action<TEvent> handler) => _handler == handler;
}
