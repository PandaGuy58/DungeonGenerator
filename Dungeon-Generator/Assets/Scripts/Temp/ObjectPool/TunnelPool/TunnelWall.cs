using UnityEngine;

public class TunnelWall : ObjectPoolMasterclass
{
    public static TunnelWall instance;

    private void Awake()
    {
        instance = this;
    }
}
