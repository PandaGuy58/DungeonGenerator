using UnityEngine;

public class DungeonMinorColumn : ObjectPoolMasterclass
{
    public static DungeonMinorColumn instance;

    private void Awake()
    {
        instance = this;
    }
}
