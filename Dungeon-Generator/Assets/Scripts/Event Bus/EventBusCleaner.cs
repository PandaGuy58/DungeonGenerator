using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Opcjonalny helper – automatycznie czyści wybrane kanały EventBusa
/// przy przeładowaniu sceny, żeby uniknąć memory leaków z nieodpisanych
/// subskrybentów (np. gdy obiekty są niszczone bez wywołania OnDisable).
///
/// Dodaj ten komponent do jednego obiektu w każdej scenie (lub do DontDestroyOnLoad).
/// Rozszerz metodę ClearAllBuses() o własne typy zdarzeń.
/// </summary>
public class EventBusCleaner : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    private void OnSceneUnloaded(Scene scene)
    {
        ClearAllBuses();
    }

    private static void ClearAllBuses()
    {
        // Dodaj tutaj wszystkie typy zdarzeń używane w grze
        EventBus<PlayerDamagedEvent>.Clear();
        EventBus<ItemPickedUpEvent>.Clear();
        EventBus<GameOverEvent>.Clear();

        Debug.Log("[EventBusCleaner] Wszystkie kanały EventBusa wyczyszczone.");
    }
}
