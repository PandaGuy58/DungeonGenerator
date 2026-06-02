using UnityEngine;

public class TempleTile : TileMasterClass
{
    private void Awake()
    {
        wallPool = TempleWall.instance;
        majorColumnPool= TempleMajorColumn.instance;
        minorColumnPool= TempleMinorColumn.instance;
        doorPool = TempleDoor.instance;

        InitialiseTile();
    }
}
