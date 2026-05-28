using UnityEngine;

public class TempleWall : ObjectPoolMasterclass
{
    public static TempleWall instance;

    private void Awake()
    {
        instance = this;
    }
}