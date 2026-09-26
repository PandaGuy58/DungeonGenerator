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
public struct TempArrayGenerationEvent : IEvent
{
    public Vector3 InitialTile;
    public Vector3 CurrentTargetTile;
    public Biome Biome;
}

public struct GenerateTilesEvent : IEvent
{
    public GenerationData[,] GenerationDataArray;
}

public struct RemoveFromTempArrayEvent : IEvent { }


public struct FinaliseArrayEvent : IEvent { }

public struct GenerateContentsEvent : IEvent
{
    public GenerationData[,] GenerationDataArray;
}

public struct DestroyContentsEvent : IEvent { }

public struct UpdateUIEvent : IEvent
{
    public string Text;
}