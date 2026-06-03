using UnityEngine;

public class TileMasterClass : MonoBehaviour
{
    [HideInInspector] public bool passive;
    [HideInInspector] public ObjectPoolMasterclass wallPool;
    [HideInInspector] public ObjectPoolMasterclass majorColumnPool;
    [HideInInspector] public ObjectPoolMasterclass minorColumnPool;
    [HideInInspector] public ObjectPoolMasterclass doorPool;

    [HideInInspector] public PoolChild poolChildReference;
    [HideInInspector] public Renderer rend;

    public void InitialiseTile()
    {
        poolChildReference = GetComponent<PoolChild>();
        rend = GetComponent<Renderer>();
    }

    public void ReturnToPool()
    {
        poolChildReference.ReturnChildToPool();
    }

    public PoolChild RequestWall()
    {
        return wallPool.RequestObject();
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
