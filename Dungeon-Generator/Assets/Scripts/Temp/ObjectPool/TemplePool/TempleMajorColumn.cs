using UnityEngine;

public class TempleMajorColumn : ObjectPoolMasterclass
{
    public static TempleMajorColumn instance;

    private void Awake()
    {
        instance = this;
    }
}