using UnityEngine;

public class DungeonWall : ObjectPoolMasterclass
{
    public static DungeonWall instance;

    private void Awake()
    {
        instance = this;
    }
}
