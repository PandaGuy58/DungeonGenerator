using UnityEngine;

// ────────────────────────────────────────────────
//  Przykładowe zdarzenia – definiuj swoje analogicznie
// ────────────────────────────────────────────────

/// <summary>Gracz otrzymał obrażenia.</summary>
public struct PlayerDamagedEvent : IEvent
{
    public int Damage;
    public Vector3 HitPosition;
}

/// <summary>Gracz zebrał przedmiot.</summary>
public struct ItemPickedUpEvent : IEvent
{
    public string ItemId;
    public int Quantity;
}

/// <summary>Gra się skończyła.</summary>
public struct GameOverEvent : IEvent
{
    public bool PlayerWon;
    public float ElapsedTime;
}

// my code 
public struct ExecuteTempArrayGenerationEvent : IEvent
{
    public Vector3 initialTile;
    public Vector3 currentTargetTile;
    public Biome biome;
    public ExecuteTempArrayGenerationEvent(Vector3 initialTile, Vector3 currentTargetTile, Biome biome)
    {
        this.initialTile = initialTile;
        this.currentTargetTile = currentTargetTile;
        this.biome = biome;
    }    
}

public struct ExecuteGeneration : IEvent
{

}
