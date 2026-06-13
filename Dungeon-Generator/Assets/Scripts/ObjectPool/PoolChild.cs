using UnityEngine;

public class PoolChild : MonoBehaviour
{
    ObjectPoolMasterclass pool;
    [SerializeField] bool destructive;

    [SerializeField] public string columnName;

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
