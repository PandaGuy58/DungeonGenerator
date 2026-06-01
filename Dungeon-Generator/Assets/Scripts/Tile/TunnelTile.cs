
public class TunnelTile : TileMasterClass
{
    private void Awake()
    {
        passive = true;
        wallPool = TunnelWall.instance;
        majorColumnPool = TunnelMajorColumn.instance;
        minorColumnPool = TunnelMinorColumn.instance;

        poolChildReference = GetComponent<PoolChild>();
    }
}
