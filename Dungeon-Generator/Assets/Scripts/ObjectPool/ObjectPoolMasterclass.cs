using UnityEngine;
using System.Collections.Generic;

public class ObjectPoolMasterclass : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    List<PoolChild> availableInstances = new List<PoolChild>();

    public PoolChild RequestObject()
    {
        if(availableInstances.Count == 0)
        {
            PoolChild poolChild = Instantiate(prefab, Vector3.zero, Quaternion.identity).GetComponent<PoolChild>();
            poolChild.Initialise(this);
            return poolChild;
        }

        PoolChild temp = availableInstances[0];
        temp.gameObject.SetActive(true);
        availableInstances.RemoveAt(0);
        return temp;
    }

    public void ReturnObject(PoolChild targetObject)
    {
        availableInstances.Add(targetObject);
        targetObject.gameObject.SetActive(false);
    }

}
