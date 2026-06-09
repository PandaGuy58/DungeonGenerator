using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public enum TileType
{
    Dungeon,
    Library,
    Temple,
    Tunnel
}

public class TileMasterClass : MonoBehaviour
{
    [HideInInspector] public ObjectPoolMasterclass wallPool { get; private set; }
    [HideInInspector] public ObjectPoolMasterclass majorColumnPool { get; private set; }
    [HideInInspector] public ObjectPoolMasterclass minorColumnPool { get; private set; }
    [HideInInspector] public ObjectPoolMasterclass doorPool { get; private set; }
    [HideInInspector] public TileType tileType { get; private set; }

    [SerializeField] Renderer rend; // serialized in case the shader is nested in the gameobject's hierarchy

    public void SetWallPool(ObjectPoolMasterclass wallPool)
    {
        this.wallPool = wallPool;
    }

    public void SetMajorColumnPool(ObjectPoolMasterclass majorColumnPool)
    {
        this.majorColumnPool = majorColumnPool;
    }

    public void SetMinorColumnPool(ObjectPoolMasterclass minorColumnPool)
    {
        this.minorColumnPool = minorColumnPool;
    }

    public void SetDoorPool(ObjectPoolMasterclass doorPool)
    {
        this.doorPool = doorPool;
    }

    public void SetTileType(TileType tileType)
    {
        this.tileType = tileType;
    }
    public void ControlShader(bool shaderActive)
    {
        rend.enabled = shaderActive;

    }
}
       