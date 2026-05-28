using UnityEngine;

public class TempleMinorColumn : ObjectPoolMasterclass
{
    public static TempleMinorColumn instance;

    private void Awake()
    {
        instance = this;
    }
}