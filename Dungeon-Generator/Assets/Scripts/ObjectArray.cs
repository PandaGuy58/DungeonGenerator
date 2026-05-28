using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectArray : MonoBehaviour
{
    public static ObjectArray instance;

    PoolChild[,] poolChildArray;

    List<PoolChild> poolChildList = new List<PoolChild>();

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

    public void GenerateContent()
    {
        ReturnContentToPools();
        GenerateWalls();
        GenerateWalls();
        GenerateMajorColumns();
        GenerateMinorColumns();
        GenerateDoors();
    }

    void ReturnContentToPools()
    {
        for(int i = 0; i < poolChildList.Count; i++)
        {
            poolChildList[i].ReturnChildToPool();
        }
    }

    void GenerateWalls()
    {

    }

    void GenerateMajorColumns()
    {

    }

    void GenerateMinorColumns()
    {

    }

    void GenerateDoors()
    {

    }
}

