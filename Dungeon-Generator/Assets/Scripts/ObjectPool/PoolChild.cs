using UnityEngine;

public class PoolChild : MonoBehaviour
{
    ObjectPoolMasterclass pool;
    [SerializeField] bool destructive;

    public void Initialise(ObjectPoolMasterclass pool)
    {
        this.pool = pool;
    }

    public void ReturnChildToPool()
    {
        pool.ReturnObject(this);
    }

    public bool IsDestructive()
    {
        return destructive;
    }
}
