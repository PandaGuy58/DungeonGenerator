using UnityEngine;

public class LibraryTile : TileMasterClass
{
    void Awake()
    {
        wallPool = LibraryWall.instance;
        majorColumnPool = LibraryMajorColumn.instance;
        minorColumnPool = LibraryMinorColumn.instance;
        doorPool = LibraryDoor.instance;
    }

}
