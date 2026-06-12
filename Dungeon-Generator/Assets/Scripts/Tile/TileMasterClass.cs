using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public enum TileType
{
    Dungeon,
    Library,
    Temple,
    Tunnel,
    Null
}

public class TileMasterClass : MonoBehaviour
{
    [HideInInspector] public ObjectPoolMasterclass wallPool { get; private set; }
    [HideInInspector] public ObjectPoolMasterclass majorColumnPool { get; private set; }
    [HideInInspector] public ObjectPoolMasterclass minorColumnPool { get; private set; }
    [HideInInspector] public ObjectPoolMasterclass doorPool { get; private set; }
    [HideInInspector] public TileType tileType { get; private set; }

    [SerializeField] Renderer rend; // serialized in case the shader is nested in the gameobject's hierarchy

    [SerializeField] public bool topWall;    // { get; private set; }
    [SerializeField] public bool bottomWall; // { get; private set; }
    [SerializeField] public bool leftWall;   // { get; private set; }
    [SerializeField] public bool rightWall;           // { get; private set; }

    [SerializeField] public bool topLeftColumn;
    [SerializeField] public bool topRightColumn;
    [SerializeField] public bool bottomLeftColumn;
    [SerializeField] public bool bottomRightColumn;

    [SerializeField] public int x;
    [SerializeField] public int y;

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

    public void ResetWallsColumns()
    {
        topWall = false;
        bottomWall = false;
        rightWall = false;
        leftWall = false;

        topLeftColumn = false;
        topRightColumn = false;
        bottomLeftColumn = false;
        bottomRightColumn = false;
    }

    public void SetTopWall()
    {
        topWall = true;
    }

    public void SetBottomWall()
    {
        bottomWall = true;
    }

    public void SetLeftWall()
    {
        leftWall = true;
    }

    public void SetRightWall()
    {
        rightWall = true;
    }

    public void SetXY(int  x, int y)
    {
        this.x = x;
        this.y = y;
    }
}
       