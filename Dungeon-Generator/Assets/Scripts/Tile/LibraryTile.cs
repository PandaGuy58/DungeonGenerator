public class LibraryTile : TileMasterClass
{
    private void Awake()
    {
        SetWallPool(LibraryWall.instance);
        SetMajorColumnPool(LibraryMajorColumn.instance);
        SetMinorColumnPool(LibraryMinorColumn.instance);
        SetDoorPool(LibraryDoor.instance);
        SetTileType(TileType.Library);
    }

}
