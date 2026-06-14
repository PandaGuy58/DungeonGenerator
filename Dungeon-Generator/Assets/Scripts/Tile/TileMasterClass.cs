using UnityEngine;

public enum TileType
{
    Dungeon,
    Library,
    Temple,
    Tunnel,
}

public class TileMasterClass : MonoBehaviour
{
    [HideInInspector] public ObjectPoolMasterclass wallPool { get; private set; }
    [HideInInspector] public ObjectPoolMasterclass majorColumnPool { get; private set; }
    [HideInInspector] public ObjectPoolMasterclass minorColumnPool { get; private set; }
    [HideInInspector] public ObjectPoolMasterclass doorPool { get; private set; }
    [HideInInspector] public TileType tileType { get; private set; }

    [SerializeField] Renderer rend; // serialized in case the shader is nested in the gameobject's hierarchy

    [HideInInspector] public bool topWall { get; private set; }
    [HideInInspector] public bool bottomWall { get; private set; }
    [HideInInspector] public bool leftWall { get; private set; }
    [HideInInspector] public bool rightWall { get; private set; }

    [HideInInspector] public bool topLeftColumn { get; private set; }
    [HideInInspector] public bool topRightColumn { get; private set; }
    [HideInInspector] public bool bottomLeftColumn { get; private set; }
    [HideInInspector] public bool bottomRightColumn { get; private set; }


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
        if(shaderActive)
        {
            rend.material.SetFloat("_Active", 1);
        }
        else
        {
            rend.material.SetFloat("_Active", 0);
        }
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

    public void SetTopRightColumn()
    {
        topRightColumn = true;
    }

    public void SetTopLeftColumn()
    {
        topLeftColumn = true;
    }

    public void SetBottomLeftColumn()
    {
        bottomLeftColumn = true;
    }

    public void SetBottomRightColumn()
    {
        bottomRightColumn = true;
    }
}
       