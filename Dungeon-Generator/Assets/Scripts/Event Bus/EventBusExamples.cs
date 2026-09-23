using System;
using UnityEngine;

// ════════════════════════════════════════════════════════
//  PUBLISHER – komponent publikujący zdarzenia
// ════════════════════════════════════════════════════════
public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 100;
    private int _currentHealth;

    private void Awake() => _currentHealth = _maxHealth;

    public void TakeDamage(int damage, Vector3 hitPos)
    {
        _currentHealth = Mathf.Max(0, _currentHealth - damage);

        // Publikacja zdarzenia – żaden subscriber nie musi tu być znany
        EventBus<PlayerDamagedEvent>.Publish(new PlayerDamagedEvent
        {
            Damage      = damage,
            HitPosition = hitPos
        });

        if (_currentHealth == 0)
            EventBus<GameOverEvent>.Publish(new GameOverEvent { PlayerWon = false });
    }
}


// ════════════════════════════════════════════════════════
//  SUBSCRIBER – komponent nasłuchujący zdarzeń
// ════════════════════════════════════════════════════════
public class UIHealthBar : MonoBehaviour
{
    // Trzymamy referencję do handlera, żeby móc go poprawnie wypisać
    private Action<PlayerDamagedEvent> _onPlayerDamaged;

    private void OnEnable()
    {
        _onPlayerDamaged = OnPlayerDamaged;
        EventBus<PlayerDamagedEvent>.Subscribe(_onPlayerDamaged);
    }

    private void OnDisable()
    {
        EventBus<PlayerDamagedEvent>.Unsubscribe(_onPlayerDamaged);
    }

    private void OnPlayerDamaged(PlayerDamagedEvent evt)
    {
        Debug.Log($"[UIHealthBar] Gracz otrzymał {evt.Damage} obrażeń w {evt.HitPosition}");
        // Aktualizuj UI...
    }
}


// ════════════════════════════════════════════════════════
//  SUBSCRIBER Z WŁASNYM BINDINGIEM (zaawansowany wzorzec)
// ════════════════════════════════════════════════════════
public class GameOverHandler : MonoBehaviour
{
    // Własny binding – możesz rozszerzyć EventBinding o priorytet, filtry itp.
    private readonly EventBinding<GameOverEvent> _binding
        = new EventBinding<GameOverEvent>(null); // handler ustawiamy niżej

    private void Awake()
    {
        // Możesz też przekazać handler w konstruktorze
    }

    private void OnEnable()
    {
        EventBus<GameOverEvent>.Subscribe(HandleGameOver);
    }

    private void OnDisable()
    {
        EventBus<GameOverEvent>.Unsubscribe(HandleGameOver);
    }

    private void HandleGameOver(GameOverEvent evt)
    {
        Debug.Log(evt.PlayerWon ? "Wygrana!" : "Przegrana!");
        // Pokaż ekran końca gry...
    }
}
