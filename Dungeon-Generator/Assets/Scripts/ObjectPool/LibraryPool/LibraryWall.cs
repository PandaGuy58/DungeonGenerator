using UnityEngine;

public class LibraryWall : ObjectPoolMasterclass
{
    public static LibraryWall instance;

    private void Awake()
    {
        instance = this;
    }
}