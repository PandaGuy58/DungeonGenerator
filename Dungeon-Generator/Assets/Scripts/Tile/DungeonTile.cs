public class DungeonTile : TileMasterClass
{
    private void Awake()
    {
        SetWallPool(DungeonWall.instance);
        SetMajorColumnPool(DungeonMajorColumn.instance);
        SetMinorColumnPool(DungeonMinorColumn.instance);
        SetDoorPool(DungeonDoor.instance);
        InitialiseTile();
    }
}
