using UnityEngine;

public class LibraryMajorColumn : ObjectPoolMasterclass
{
    public static LibraryMajorColumn instance;

    private void Awake()
    {
        instance = this;
    }
}