using UnityEngine;

public class Tunnel : TileMasterClass
{

    private void Awake()
    {
        passive = true;
        wallPool = TunnelWall.instance;
        majorColumnPool = TunnelMajorColumn.instance;
        minorColumnPool = TunnelMinorColumn.instance;
    }
}
