using UnityEngine;

public class TileMasterClass : MonoBehaviour
{
    [HideInInspector] public ObjectPoolMasterclass wallPool {  get; private set; }
    [HideInInspector] public ObjectPoolMasterclass majorColumnPool { get; private set; }
    [HideInInspector] public ObjectPoolMasterclass minorColumnPool { get; private set; }
    [HideInInspector] public ObjectPoolMasterclass doorPool { get; private set; }

    [HideInInspector] PoolChild poolChildReference;
    [HideInInspector] Renderer rend;

    public void InitialiseTile()
    {
        poolChildReference = GetComponent<PoolChild>();
        rend = GetComponent<Renderer>();
    }

    public void SetWallPool(ObjectPoolMasterclass wallPool)
    {
        this.wallPool = wallPool;
    }

    public void SetMajorColumnPool(ObjectPoolMasterclass majorColumnPool)
    {
        this.majorColumnPool = majorColumnPool;
    }

    public void SetMinorColumnPool(ObjectPoolMasterclass minorColumnPool)
    {
        this.minorColumnPool = minorColumnPool;
    }

    public void SetDoorPool(ObjectPoolMasterclass doorPool)
    {
        this.doorPool = doorPool;
    }

    public void ReturnToPool()
    {
        poolChildReference.ReturnChildToPool();
    }

    public void ControlShader(bool shaderActive)
    {
        if (shaderActive)
        {
            rend.material.SetFloat("_Active", 1);
        }
        else
        {
            rend.material.SetFloat("_Active", 0);
        }        
    }
}
