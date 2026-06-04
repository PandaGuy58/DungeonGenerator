public class TempleTile : TileMasterClass
{
    private void Awake()
    {
        SetWallPool(TempleWall.instance);
        SetMajorColumnPool(TempleMajorColumn.instance);
        SetMinorColumnPool(TempleMinorColumn.instance);
        SetDoorPool(TempleDoor.instance);
        InitialiseTile();
    }
}
