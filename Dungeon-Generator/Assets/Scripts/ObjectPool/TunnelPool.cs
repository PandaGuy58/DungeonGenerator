using UnityEngine;

public class TunnelPool : ObjectPoolMasterclass
{
    public static TunnelPool instance;

    private void Awake()
    {
        instance = this;
    }
}
