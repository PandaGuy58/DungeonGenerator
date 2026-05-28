using UnityEngine;

public class TileMasterClass : MonoBehaviour
{
    [HideInInspector] public bool passive;
    [HideInInspector] public ObjectPoolMasterclass wallPool;
    [HideInInspector] public ObjectPoolMasterclass majorColumnPool;
    [HideInInspector] public ObjectPoolMasterclass minorColumnPool;
    [HideInInspector] public ObjectPoolMasterclass doorPool;  
}
