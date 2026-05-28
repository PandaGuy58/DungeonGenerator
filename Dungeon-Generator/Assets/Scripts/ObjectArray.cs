using System;
using UnityEngine;

public class ObjectArray : MonoBehaviour
{
    public static ObjectArray instance;

    PoolChild[,] poolChildArray;

    private void Awake()
    {
        instance = this;
        poolChildArray = new PoolChild[51, 51];
    }

    public void AssignObjectToArray(PoolChild child, int x, int y)
    {
        Debug.Log(x + "," + y);
        if (poolChildArray[x, y] != null)
        {
            Debug.Log("Location already occupied" + Time.time);
            poolChildArray[x, y].ReturnChildToPool();
        }
        poolChildArray[x, y] = child;
    }

    public void ReleaseObjectFromArray(int x, int y)
    {
        poolChildArray[x, y].ReturnChildToPool();
        poolChildArray[x, y] = null;
    }
}

    /*
    public void DisplayArray()
    {
        for (int y = 0; y < 25; y++)
        {
            string finalString = "";
            for (int x = 0; x < 25; x++)
            {
                if (poolChildArray[x, y] == null)
                {
                    finalString += "NULL";
                }
                else
                {
                    finalString += poolChildArray[x, y].ToString();
                }

                finalString += ",";
            }
            Debug.Log(finalString);
        }
    }

}
    */