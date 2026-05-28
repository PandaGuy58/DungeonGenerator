
public class LibraryTile : TileMasterClass
{
    private void Awake()
    {
        wallPool = LibraryWall.instance;
        majorColumnPool = LibraryMajorColumn.instance;
        minorColumnPool = LibraryMinorColumn.instance;
        doorPool = LibraryDoor.instance;

        poolChildReference = GetComponent<PoolChild>();
    }

}
