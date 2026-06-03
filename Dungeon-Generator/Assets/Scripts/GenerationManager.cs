using NUnit.Framework.Constraints;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Tilemaps.Tilemap;

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
                tileArray[x,z] = tile;

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
        for(int x = 0; x < tileArray.GetLength(0); x++)
        {
            for(int z = 0; z < tileArray.GetLength(1); z++)
            {
                if (tileArray[x, z] == null)
                    continue;

                GenerateTopWall(x, z);
                GenerateBottomWall(x, z);
                GenerateRightWall(x, z);
                GenerateLeftWall(x, z);
            }
        }
    }

    void GenerateTopWall(int x, int y)
    {
        if (tileArray[x, y + 1] != null)
            return;

        Vector3 calculate = tileArray[x, y].transform.position;
        calculate.z += 1;
        calculate.y += 0.5f;
        calculate.x -= 0.5f;

        PoolChild poolChild = tileArray[x, y].RequestWall();
        poolChild.gameObject.transform.position = calculate;

        calculate = Vector3.zero;
        calculate.y = 90;
        poolChild.gameObject.transform.eulerAngles = calculate;
        contents.Add(poolChild);
    }

    void GenerateBottomWall(int x, int y)
    {
        if (tileArray[x, y - 1] != null)
            return;

        Vector3 calculate = tileArray[x, y].transform.position;
        calculate.y += 0.5f;
        calculate.x -= 0.5f;

        PoolChild poolChild = tileArray[x, y].RequestWall();
        poolChild.gameObject.transform.position = calculate;

        calculate = Vector3.zero;
        calculate.y = 90;
        poolChild.gameObject.transform.eulerAngles = calculate;
        contents.Add(poolChild);

    }

    void GenerateRightWall(int x, int y)
    {
        if (tileArray[x + 1, y] != null)
            return;

        Vector3 calculate = tileArray[x, y].transform.position;
        calculate.y += 0.5f;
        calculate.z += 0.5f;

        PoolChild poolChild = tileArray[x, y].RequestWall();
        poolChild.gameObject.transform.position = calculate;

        calculate = Vector3.zero;
        poolChild.gameObject.transform.eulerAngles = calculate;
        contents.Add(poolChild);
    }

    void GenerateLeftWall(int x, int y)
    {
        if (tileArray[x - 1, y] != null)
            return;

        Vector3 calculate = tileArray[x, y].transform.position;
        calculate.y += 0.5f;
        calculate.x -= 1;
        calculate.z += 0.5f;

        PoolChild poolChild = tileArray[x, y].RequestWall();
        poolChild.gameObject.transform.position = calculate;

        calculate = Vector3.zero;
        poolChild.gameObject.transform.eulerAngles = calculate;
        contents.Add(poolChild);
    }



}




    /*
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
}
    */







                /*

            }
        }
    }
}

                */



                /*
                if (!dataArray[x, z].destruction)
                    return;

                TileMasterClass tile = newTile.GetComponent<TileMasterClass>();
                tile.ControlShader(true);
            }
        }
    }
}

              
           
                 */