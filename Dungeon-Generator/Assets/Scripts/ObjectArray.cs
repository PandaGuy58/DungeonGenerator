using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectArray : MonoBehaviour
{
    public static ObjectArray instance;

    TileMasterClass[,]tilesArray;

    List<PoolChild> poolChildList = new List<PoolChild>();

    private void Awake()
    {
        instance = this;
        tilesArray = new TileMasterClass[51, 51];
    }

    public void AssignObjectToArray(TileMasterClass child, int x, int y)
    {
        Debug.Log(x + "," + y);
        if (tilesArray[x, y] != null)
        {
            Debug.Log("Location already occupied" + Time.time);
            tilesArray[x, y].ReturnToPool();
        }
        tilesArray[x, y] = child;
    }

    public void ReleaseObjectFromArray(int x, int y)
    {
        tilesArray[x, y].ReturnToPool();
        tilesArray[x, y] = null;
    }

    public void GenerateContent()
    {
        ReturnContentToPools();
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
        for (int x = 0; x < tilesArray.GetLength(0); x++)
        {
            for (int y = 0; y < tilesArray.GetLength(1); y++)
            {
                if (tilesArray[x, y] == null)
                    continue;

                GenerateTopWall(x, y);
                GenerateBottomWall(x, y);
                GenerateRightWall(x, y);
                GenerateLeftWall(x, y);
            }
        }
    }

    void GenerateTopWall(int x, int y)
    {
        if (tilesArray[x, y + 1] != null)
            return;

        Vector3 calculate = tilesArray[x, y].transform.position;
        calculate.z += 0.5f;

        PoolChild poolChild = tilesArray[x, y].RequestWall();
        poolChild.gameObject.transform.position = calculate;

        calculate = Vector3.zero;
        calculate.y = 90;
        poolChild.gameObject.transform.eulerAngles = calculate;
        poolChildList.Add(poolChild);
    }

    void GenerateBottomWall(int x, int y)
    {
        if (tilesArray[x, y - 1] != null)
            return;

        Vector3 calculate = tilesArray[x, y].transform.position;
        calculate.z -= 0.5f;

        PoolChild poolChild = tilesArray[x, y].RequestWall();
        poolChild.gameObject.transform.position = calculate;

        calculate = Vector3.zero;
        calculate.y = 90;
        poolChild.gameObject.transform.eulerAngles = calculate;
        poolChildList.Add(poolChild);
    }

    void GenerateRightWall(int x, int y)
    {
        if (tilesArray[x + 1, y] != null)
            return;

        Vector3 calculate = tilesArray[x, y].transform.position;
        calculate.x += 0.5f;

        PoolChild poolChild = tilesArray[x, y].RequestWall();
        poolChild.gameObject.transform.position = calculate;

        calculate = Vector3.zero;
        poolChild.gameObject.transform.eulerAngles = calculate;
        poolChildList.Add(poolChild);

    }

    void GenerateLeftWall(int x, int y)
    {
        if (tilesArray[x - 1, y] != null)
            return;

        Vector3 calculate = tilesArray[x, y].transform.position;
        calculate.x -= 0.5f;

        PoolChild poolChild = tilesArray[x, y].RequestWall();
        poolChild.gameObject.transform.position = calculate;

        calculate = Vector3.zero;
        poolChild.gameObject.transform.eulerAngles = calculate;
        poolChildList.Add(poolChild);
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

