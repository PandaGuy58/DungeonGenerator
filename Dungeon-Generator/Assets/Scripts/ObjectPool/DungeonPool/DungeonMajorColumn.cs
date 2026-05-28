using UnityEngine;

public class DungeonMajorColumn : ObjectPoolMasterclass
{
    public static DungeonMajorColumn instance;

    private void Awake()
    {
        instance = this;
    }
}

