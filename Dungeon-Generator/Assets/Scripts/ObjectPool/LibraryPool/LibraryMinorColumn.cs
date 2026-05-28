using UnityEngine;

public class LibraryMinorColumn : ObjectPoolMasterclass
{
    public static LibraryMinorColumn instance;

    private void Awake()
    {
        instance = this;
    }
}