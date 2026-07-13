using UnityEngine;

[CreateAssetMenu(fileName = "Biome", menuName = "Scriptable Objects/Biome")]
public class Biome : ScriptableObject
{
    [SerializeField] private bool isDestructive;
    [SerializeField] private bool stopWallGeneration;
    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private GameObject wallPrefab;

    public bool IsDestructive() => isDestructive;
    public bool StopWallGeneration() => stopWallGeneration;
    public GameObject FloorPrefab() => floorPrefab;
    public GameObject WallPrefab() => wallPrefab;
}
