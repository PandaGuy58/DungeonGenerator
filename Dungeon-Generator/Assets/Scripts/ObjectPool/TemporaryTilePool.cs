using UnityEngine;

public class TemporaryTilePool : ObjectPoolMasterclass
{
    public static TemporaryTilePool instance;

    private void Awake()
    {
        instance = this;
    }
}
