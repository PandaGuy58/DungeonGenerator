using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

    Dictionary<GameObject, int> poolDictionary = new Dictionary<GameObject, int>();
    List<List<PoolChild>> gameObjectLists = new List<List<PoolChild>>();
    List<Transform> objectParents = new List<Transform>();

    private void Awake()
    {
        instance = this;
    }

    public PoolChild GetInstance(GameObject prefab)
    {
        int index;
        if (!poolDictionary.ContainsKey(prefab))
        {
            index = gameObjectLists.Count;
            GenerateNewPool(prefab, index);
        }
        else
        {
            index = poolDictionary[prefab];
        }

        return GenerateInstance(index, prefab);
    }

    public void ReturnInstance(PoolChild instance)
    {
        gameObjectLists[instance.id].Add(instance);
        instance.gameObject.SetActive(false);
    }

    public void ClearPool(GameObject prefab)
    {
        int index = poolDictionary[prefab];
        gameObjectLists[index].Clear();
    }

    PoolChild GenerateInstance(int index, GameObject prefab)
    {
        if (gameObjectLists[index].Count == 0)
        {
            GameObject newInstance = Instantiate(prefab);
            PoolChild poolChild = newInstance.AddComponent<PoolChild>();
            poolChild.Initialise(index);
            poolChild.transform.parent = objectParents[index];
            return poolChild;
        }
        else
        {
            List<PoolChild> targetList = gameObjectLists[index];
            int itemIndex = gameObjectLists[index].Count - 1;
            PoolChild temp = targetList[itemIndex];
            targetList.RemoveAt(itemIndex);
            temp.gameObject.SetActive(true);
            return temp;
        }
    }

    void GenerateNewPool(GameObject prefab, int index)
    {
        poolDictionary.Add(prefab, index);
        gameObjectLists.Add(new List<PoolChild>());

        Transform newTransform = new GameObject().transform;
        newTransform.name = "Pool: " + prefab.name;
        objectParents.Add(newTransform);
        newTransform.parent = transform;
    }
}

/*
 * PoolChild GenerateInstance(int index, GameObject prefab)
{
    if (gameObjectLists[index].Count == 0)
    {
        GameObject newInstance = Instantiate(prefab);

        PoolChild poolChild = newInstance.AddComponent<PoolChild>();
        poolChild.Initialise(index);

        return poolChild;
    }

    List<PoolChild> targetList = gameObjectLists[index];
    int itemIndex = targetList.Count - 1;

    PoolChild temp = targetList[itemIndex];
    targetList.RemoveAt(itemIndex);

    temp.gameObject.SetActive(true);
    return temp;
}
*/