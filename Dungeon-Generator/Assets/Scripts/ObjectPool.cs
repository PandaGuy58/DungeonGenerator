using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool _instance;

    Dictionary<GameObject, int> _poolDictionary = new Dictionary<GameObject, int>();
    List<List<PoolChild>> _gameObjectLists = new List<List<PoolChild>>();
    List<Transform> _objectParents = new List<Transform>();

    private void Awake()
    {
        _instance = this;
    }

    public PoolChild GetInstance(GameObject prefab)
    {
        int index;
        if (!_poolDictionary.ContainsKey(prefab))
        {
            index = _gameObjectLists.Count;
            GenerateNewPool(prefab, index);
        }
        else
        {
            index = _poolDictionary[prefab];
        }

        return GenerateInstance(index, prefab);
    }

    public void ReturnInstance(PoolChild instance)
    {
        _gameObjectLists[instance.id].Add(instance);
        instance.gameObject.SetActive(false);
    }

    public void ClearPool(GameObject prefab)
    {
        int index = _poolDictionary[prefab];
        _gameObjectLists[index].Clear();
    }

    PoolChild GenerateInstance(int index, GameObject prefab)
    {
        if (_gameObjectLists[index].Count == 0)
        {
            GameObject newInstance = Instantiate(prefab);
            PoolChild poolChild = newInstance.AddComponent<PoolChild>();
            poolChild.Initialise(index);
            poolChild.transform.parent = _objectParents[index];
            return poolChild;
        }
        else
        {
            List<PoolChild> targetList = _gameObjectLists[index];
            int itemIndex = _gameObjectLists[index].Count - 1;
            PoolChild temp = targetList[itemIndex];
            targetList.RemoveAt(itemIndex);
            temp.gameObject.SetActive(true);
            return temp;
        }
    }

    void GenerateNewPool(GameObject prefab, int index)
    {
        _poolDictionary.Add(prefab, index);
        _gameObjectLists.Add(new List<PoolChild>());
        Transform newTransform = new GameObject().transform;
        newTransform.name = "Pool: " + prefab.name;
        _objectParents.Add(newTransform);
        newTransform.parent = transform;
    }
}