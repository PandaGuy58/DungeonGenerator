using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Globalny, generyczny EventBus oparty na typach zdarzeń.
/// Użycie: EventBus<MojeZdarzenie>.Subscribe(Handler);
///         EventBus<MojeZdarzenie>.Publish(new MojeZdarzenie(...));
///         EventBus<MojeZdarzenie>.Unsubscribe(Handler);
/// </summary>
public static class EventBus<TEvent> where TEvent : IEvent
{
    private static readonly HashSet<IEventBinding<TEvent>> _bindings = new();

    public static void Subscribe(Action<TEvent> handler)
    {
        var binding = new EventBinding<TEvent>(handler);
        _bindings.Add(binding);
    }

    public static void Subscribe(IEventBinding<TEvent> binding)
    {
        _bindings.Add(binding);
    }

    public static void Unsubscribe(Action<TEvent> handler)
    {
        _bindings.RemoveWhere(b => b.MatchesHandler(handler));
    }

    public static void Unsubscribe(IEventBinding<TEvent> binding)
    {
        _bindings.Remove(binding);
    }

    public static void Publish(TEvent @event)
    {
        // Kopia kolekcji, żeby uniknąć błędów przy modyfikacji podczas iteracji
        foreach (var binding in new HashSet<IEventBinding<TEvent>>(_bindings))
        {
            try
            {
                binding.Invoke(@event);
            }
            catch (Exception e)
            {
                Debug.LogError($"[EventBus] Błąd w handlerze zdarzenia {typeof(TEvent).Name}: {e}");
            }
        }
    }

    /// <summary>Usuwa wszystkich subskrybentów – użyj ostrożnie (np. przy przeładowaniu sceny).</summary>
    public static void Clear() => _bindings.Clear();
}
