using UnityEngine;

public class PoolChild : MonoBehaviour
{
    ObjectPoolMasterclass pool;

    public void Initialise(ObjectPoolMasterclass pool)
    {
        this.pool = pool;
    }

    public void ReturnChildToPool()
    {
        pool.ReturnObject(this);
    }
}
