using UnityEngine;

public class ChamberPool : ObjectPoolMasterclass
{
    public static ChamberPool instance;

    private void Awake()
    {
        instance = this;
    }
}
