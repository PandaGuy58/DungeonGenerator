using UnityEngine;

public class LibraryDoor : ObjectPoolMasterclass
{
    public static LibraryDoor instance;

    private void Awake()
    {
        instance = this;
    }
}
