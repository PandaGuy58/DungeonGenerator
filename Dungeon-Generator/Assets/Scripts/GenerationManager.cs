using System.Collections.Generic;
using UnityEngine;

public class GenerationManager : MonoBehaviour
{
    public static GenerationManager instance;
    List<PoolChild> generatedTiles = new List<PoolChild>();
    List<PoolChild> contents = new List<PoolChild>();

    TileMasterClass[,] tileArray;

    private void Awake()
    {
        instance = this;
    }

    public void GenerateTiles()
    {
        GenerationData[,] array = ObjectArray.instance.RequestTemporaryArray();
        ReturnToPool(generatedTiles);
        GenerateTiles(array);
    }

    public void DisableContents()
    {
        ReturnToPool(contents);
    }

    void ReturnToPool(List<PoolChild> poolChildList)
    {
        for (int i = 0; i < poolChildList.Count; i++)
        {
            poolChildList[i].ReturnChildToPool();
        }
        poolChildList.Clear();
    }

    void GenerateTiles(GenerationData[,] dataArray)
    {
        tileArray = new TileMasterClass[51, 51];
        for (int x = 0; x < dataArray.GetLength(0); x++)
        {
            for (int z = 0; z < dataArray.GetLength(1); z++)
            {
                if (dataArray[x, z] == null)
                    continue;

                PoolChild newTile = dataArray[x, z].pool.RequestObject();
                newTile.transform.position = new Vector3(x + 0.5f, 0.1f, z - 0.5f);
                generatedTiles.Add(newTile);

                if (newTile.CompareTag("Destructive"))
                    continue;

                TileMasterClass tile = newTile.GetComponent<TileMasterClass>();
                tileArray[x, z] = tile;

                if (dataArray[x, z].destruction)
                {
                    tile.ControlShader(true);
                }
                else
                {
                    tile.ControlShader(false);
                }

            }
        }
    }

    public void GenerateContents()
    {
        for (int x = 0; x < tileArray.GetLength(0); x++)
        {
            for (int z = 0; z < tileArray.GetLength(1); z++)
            {
                if (tileArray[x, z] == null)
                    continue;

                GenerateTopWall(x, z);
                GenerateBottomWall(x, z);
                GenerateRightWall(x, z);
                GenerateLeftWall(x, z);

                GenerateTopLeftCorner(x, z);
                GenerateTopRightCorner(x, z);
                GenerateBottomLeftCorner(x, z);
                GenerateBottomRightCorner(x, z);

                GenerateTopColumn(x, z);
                GenerateBottomColumn(x, z);
                GenerateLeftColumn(x, z);
                GenerateRightColumn(x, z);
            }
        }
    }

    void GenerateTopWall(int x, int y)
    {
        if (tileArray[x, y + 1] != null)
            return;

        Vector3 calculate = tileArray[x, y].transform.position;
        PoolChild poolChild = tileArray[x, y].wallPool.RequestObject();
        poolChild.gameObject.transform.position = calculate;

        calculate = Vector3.zero;
        poolChild.transform.eulerAngles = calculate;
        contents.Add(poolChild);
    }

    void GenerateBottomWall(int x, int y)
    {
        if (tileArray[x, y - 1] != null)
            return;

        Vector3 calculate = tileArray[x, y].transform.position;
        calculate.x -= 1;
        calculate.z += 1;
        PoolChild poolChild = tileArray[x, y].wallPool.RequestObject();
        poolChild.transform.position = calculate;

        calculate = Vector3.zero;
        calculate.y = 180;
        poolChild.transform.eulerAngles = calculate;
        contents.Add(poolChild);
    }

    void GenerateRightWall(int x, int y)
    {
        if (tileArray[x + 1, y] != null)
            return;

        Vector3 calculate = tileArray[x, y].transform.position;
        calculate.x -= 1;
        PoolChild poolChild = tileArray[x, y].wallPool.RequestObject();
        poolChild.transform.position = calculate;

        calculate = Vector3.zero;
        calculate.y = 90;
        poolChild.transform.eulerAngles = calculate;
        contents.Add(poolChild);
    }

    void GenerateLeftWall(int x, int y)
    {
        if (tileArray[x - 1, y] != null)
            return;

        Vector3 calculate = tileArray[x, y].transform.position;
        calculate.z += 1;
        PoolChild poolChild = tileArray[x, y].wallPool.RequestObject();
        poolChild.gameObject.transform.position = calculate;

        calculate = Vector3.zero;
        calculate.y = -90;
        poolChild.transform.eulerAngles = calculate;
        contents.Add(poolChild);
    }

    void GenerateTopLeftCorner(int x, int y)
    {
        Vector3 calculate;
        PoolChild poolChild;

        if (tileArray[x, y + 1] == null && tileArray[x - 1, y] == null)
        {
            calculate = tileArray[x, y].transform.position;
            calculate.x -= 0.85f;
            calculate.y += 0.5f;
            calculate.z += 0.85f;

            poolChild = tileArray[x, y].majorColumnPool.RequestObject();
            poolChild.transform.position = calculate;
            contents.Add(poolChild);
            return;
        }

        if (tileArray[x - 1, y + 1] != null)
            return;

        if (tileArray[x, y + 1] == null)
            return;

        if (tileArray[x - 1, y] == null)
            return;

        calculate = tileArray[x, y].transform.position;
        calculate.x -= 1;
        calculate.y += 0.5f;
        calculate.z += 1f;

        poolChild = tileArray[x, y].majorColumnPool.RequestObject();
        poolChild.transform.position = calculate;
        contents.Add(poolChild);
    }

    void GenerateTopRightCorner(int x, int y)
    {
        Vector3 calculate;
        PoolChild poolChild;

        if (tileArray[x, y + 1] == null && tileArray[x + 1, y] == null)
        {
            calculate = tileArray[x, y].transform.position;
            calculate.x -= 0.15f;
            calculate.y += 0.5f;
            calculate.z += 0.85f;

            poolChild = tileArray[x, y].majorColumnPool.RequestObject();
            poolChild.transform.position = calculate;
            contents.Add(poolChild);
            return;
        }

        if (tileArray[x + 1, y + 1] != null)
            return;

        if (tileArray[x + 1, y] == null)
            return;

        if (tileArray[x, y + 1] == null)
            return;

        calculate = tileArray[x, y].transform.position;
        calculate.y += 0.5f;
        calculate.z += 1;

        poolChild = tileArray[x, y].majorColumnPool.RequestObject();
        poolChild.transform.position = calculate;
        contents.Add(poolChild);
    }


    void GenerateBottomLeftCorner(int x, int y)
    {
        Vector3 calculate;
        PoolChild poolChild;

        if (tileArray[x - 1, y] == null && tileArray[x, y - 1] == null)
        {
            calculate = tileArray[x, y].transform.position;
            calculate.x -= 0.85f;
            calculate.y += 0.5f;
            calculate.z += 0.15f;

            poolChild = tileArray[x, y].majorColumnPool.RequestObject();
            poolChild.transform.position = calculate;
            contents.Add(poolChild);
            return;
        }

        if (tileArray[x - 1, y - 1] != null)
            return;

        if (tileArray[x - 1, y] == null)
            return;

        if (tileArray[x, y - 1] == null)
            return;

        calculate = tileArray[x, y].transform.position;
        calculate.x -= 1;
        calculate.y += 0.5f;

        poolChild = tileArray[x, y].majorColumnPool.RequestObject();
        poolChild.transform.position = calculate;
        contents.Add(poolChild);
    }

    void GenerateBottomRightCorner(int x, int y)
    {
        Vector3 calculate;
        PoolChild poolChild;

        if (tileArray[x + 1, y] == null && tileArray[x, y - 1] == null)
        {
            calculate = tileArray[x, y].transform.position;
            calculate.x -= 0.15f;
            calculate.y += 0.5f;
            calculate.z += 0.15f;

            poolChild = tileArray[x, y].majorColumnPool.RequestObject();
            poolChild.transform.position = calculate;
            contents.Add(poolChild);
        }

        if (tileArray[x + 1, y - 1] != null)
            return;

        if (tileArray[x + 1, y] == null)
            return;

        if (tileArray[x, y - 1] == null)
            return;

        calculate = tileArray[x, y].transform.position;
        calculate.y += 0.5f;

        poolChild = tileArray[x, y].majorColumnPool.RequestObject();
        poolChild.transform.position = calculate;
        contents.Add(poolChild);
    } 

    void GenerateTopColumn(int x, int y)
    {
        if (tileArray[x, y + 1] != null)
            return;

        if (tileArray[x + 1, y] == null)
            return;

        if (tileArray[x + 1, y + 1] != null)
            return;

        Vector3 calculate = tileArray[x, y].transform.position;
        calculate.y += 0.5f;
        calculate.z += 0.85f;

        PoolChild poolChild = tileArray[x, y].minorColumnPool.RequestObject();
        poolChild.transform.position = calculate;
        contents.Add(poolChild);
    }

    void GenerateBottomColumn(int x, int y)
    {
        if (tileArray[x, y - 1] != null)
            return;

        if (tileArray[x + 1, y] == null)
            return;

        if (tileArray[x + 1, y - 1] != null)
            return;

        Vector3 calculate = tileArray[x, y].transform.position;
        calculate.y += 0.5f;
        calculate.z += 0.15f;

        PoolChild poolChild = tileArray[x, y].minorColumnPool.RequestObject();
        poolChild.transform.position = calculate;
        contents.Add(poolChild);
    }

    void GenerateLeftColumn(int x, int y)
    {
        if (tileArray[x - 1, y] != null)
            return;

        if (tileArray[x, y + 1] == null)
            return;

        if (tileArray[x - 1, y + 1] != null)
            return;

        Vector3 calculate = tileArray[x, y].transform.position;
        calculate.x -= 0.85f;
        calculate.y += 0.5f;
        calculate.z += 1;

        PoolChild poolChild = tileArray[x, y].minorColumnPool.RequestObject();
        poolChild.transform.position = calculate;
        contents.Add(poolChild);
    }

    void GenerateRightColumn(int x, int y)
    {
        if (tileArray[x + 1, y] != null)
            return;

        if (tileArray[x, y + 1] == null)
            return;

        if (tileArray[x + 1, y + 1] != null)
            return;

        Vector3 calculate = tileArray[x, y].transform.position;
        calculate.x -= 0.15f;
        calculate.y += 0.5f;
        calculate.z += 0.85f;

        PoolChild poolChild = tileArray[x, y].minorColumnPool.RequestObject();
        poolChild.transform.position = calculate;
        contents.Add(poolChild);
    }
}



