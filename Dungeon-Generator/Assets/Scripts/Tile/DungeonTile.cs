using UnityEngine;

public class DungeonTile : TileMasterClass
{
    private void Awake()
    {
        wallPool = DungeonWall.instance;
        majorColumnPool = DungeonMajorColumn.instance;
        minorColumnPool = DungeonMinorColumn.instance;
        doorPool = DungeonDoor.instance;
    }
}
