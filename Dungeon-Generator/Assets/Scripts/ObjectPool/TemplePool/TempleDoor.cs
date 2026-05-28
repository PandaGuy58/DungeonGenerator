using UnityEngine;

public class TempleDoor : ObjectPoolMasterclass
{
    public static TempleDoor instance;

    private void Awake()
    {
        instance = this;
    }
}