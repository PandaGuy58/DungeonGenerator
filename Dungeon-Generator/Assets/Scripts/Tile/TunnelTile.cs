public class TunnelTile : TileMasterClass
{
    private void Awake()
    {
        SetWallPool(TunnelWall.instance);
        SetMajorColumnPool(TunnelMajorColumn.instance);
        SetMinorColumnPool(TunnelMinorColumn.instance);
        InitialiseTile();
    }
}
