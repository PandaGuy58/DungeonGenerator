using UnityEngine;

public class ObjectArray : MonoBehaviour
{
    public static ObjectArray instance;

    PoolChild[,] poolChildArray;

    private void Awake()
    {
        instance = this;
        poolChildArray = new PoolChild[25, 25];
    }

    public void AssignObjectToArray(PoolChild child, int x, int y)
    {
        poolChildArray[x,y] = child;
    }

    public void ReleaseObjectFromArray(int x, int y)
    {
        poolChildArray[x, y].ReturnChildToPool();
        poolChildArray[x, y] = null;
    }

}
