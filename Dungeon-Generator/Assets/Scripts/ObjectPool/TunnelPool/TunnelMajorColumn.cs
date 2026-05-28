using UnityEngine;

public class TunnelMajorColumn : ObjectPoolMasterclass
{
    public static TunnelMajorColumn instance;

    private void Awake()
    {
        instance = this;
    }
}
