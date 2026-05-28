using UnityEngine;

public class DungeonDoor : ObjectPoolMasterclass
{
    public static DungeonDoor instance;

    private void Awake()
    {
        instance = this;
    }
}
