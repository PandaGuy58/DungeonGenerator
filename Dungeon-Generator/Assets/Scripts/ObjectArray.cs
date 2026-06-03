using System;
using System.Collections.Generic;
using UnityEngine;

public class GenerationData
{
    public ObjectPoolMasterclass pool {  get; private set; }
    public bool destruction { get; private set; }

    public void Initialise(ObjectPoolMasterclass pool, bool destruction)
    {
        this.destruction = destruction;
        this.pool = pool;
    }

    public void EnableDestruction(bool destruction)
    {
        this.destruction = destruction;
    }
}


public class ObjectArray : MonoBehaviour
{
    public static ObjectArray instance;

    // array is larger than needed by 2 at x and y
    // x.0, y.0, x.Length, y.Length are empty for logic purposes
    // for this reason x += 1 and y += 1 are added in AssignObjectToArray & ReleaseObjectFromArray
    GenerationData[,] array;
    GenerationData[,] temporaryArray;

    // before contents are (re)generated all contents are reset
    //List<PoolChild> contentsList = new List<PoolChild>();

    private void Awake()
    {
        instance = this;
        array = new GenerationData[51, 51];

    }

    public GenerationData[,] RequestTemporaryArray()
    {
        return temporaryArray;
    }

    public void FinalisePoolArray()
    {
        array = temporaryArray;
    }

    GenerationData[,] CreateNewArray(GenerationData[,] oldArray)
    {
        GenerationData[,] newArray = new GenerationData[oldArray.GetLength(0), oldArray.GetLength(1)];
        for (int x = 0; x < oldArray.GetLength(0); x++)
        {
            for (int z = 0; z < oldArray.GetLength(1); z++)
            {
                newArray[x, z] = oldArray[x, z];
            }
        }

        return newArray;
    }

    public void GenerateTemporaryArray(Vector3 initialTile, Vector3 currentTargetTile, ObjectPoolMasterclass tilesPool)
    {
        temporaryArray = CreateNewArray(array);


        if (initialTile.x == currentTargetTile.x && initialTile.z == currentTargetTile.z)
        {
            GenerationData data = new GenerationData();
            data.Initialise(tilesPool, false);
            temporaryArray[(int)initialTile.x, (int)initialTile.z] = data;
        }
        else if (initialTile.x > currentTargetTile.x && initialTile.z == currentTargetTile.z)
        {
            for (int x = (int)initialTile.x; x > currentTargetTile.x - 1; x--)
            {
                GenerationData data = new GenerationData();
                data.Initialise(tilesPool, false);
                temporaryArray[x, (int)currentTargetTile.z] = data;
            }
        }
        else if (initialTile.x < currentTargetTile.x && initialTile.z == currentTargetTile.z)
        {
            for (int x = (int)initialTile.x; x < currentTargetTile.x + 1; x++)
            {
                GenerationData data = new GenerationData();
                data.Initialise(tilesPool, false);
                temporaryArray[x, (int)currentTargetTile.z] = data;
            }
        }
        else if (initialTile.x == currentTargetTile.x && initialTile.z < currentTargetTile.z)
        {
            for (int z = (int)initialTile.z; z < currentTargetTile.z + 1; z++)
            {
                GenerationData data = new GenerationData();
                data.Initialise(tilesPool, false);
                temporaryArray[(int)initialTile.x, z] = data;
            }
        }
        else if (initialTile.x == currentTargetTile.x && initialTile.z > currentTargetTile.z)
        {
            for (int z = (int)initialTile.z; z > currentTargetTile.z - 1; z--)
            {
                GenerationData data = new GenerationData();
                data.Initialise(tilesPool, false);
                temporaryArray[(int)initialTile.x, z] = data;
            }
        }
        else if (initialTile.x < currentTargetTile.x && initialTile.z < currentTargetTile.z)
        {
            for (int x = (int)initialTile.x; x < currentTargetTile.x + 1; x++)
            {
                for (int z = (int)initialTile.z; z < currentTargetTile.z + 1; z++)
                {
                    GenerationData data = new GenerationData();
                    data.Initialise(tilesPool, false);
                    temporaryArray[x, z] = data;
                }
            }
        }
        else if (initialTile.x > currentTargetTile.x && initialTile.z < currentTargetTile.z)
        {
            for (int x = (int)initialTile.x; x > currentTargetTile.x - 1; x--)
            {
                for (int z = (int)initialTile.z; z < currentTargetTile.z + 1; z++)
                {
                    GenerationData data = new GenerationData();
                    data.Initialise(tilesPool, false);
                    temporaryArray[x, z] = data;
                }
            }
        }
        else if (initialTile.x < currentTargetTile.x && initialTile.z > currentTargetTile.z)
        {
            for (int x = (int)initialTile.x; x < currentTargetTile.x + 1; x++)
            {
                for (int z = (int)initialTile.z; z > currentTargetTile.z - 1; z--)
                {
                    GenerationData data = new GenerationData();
                    data.Initialise(tilesPool, false);
                    temporaryArray[x, z] = data;
                }
            }
        }
        else if (initialTile.x > currentTargetTile.x && initialTile.z > currentTargetTile.z)
        {
            for (int x = (int)initialTile.x; x > currentTargetTile.x - 1; x--)
            {
                for (int z = (int)initialTile.z; z > currentTargetTile.z - 1; z--)
                {
                    GenerationData data = new GenerationData();
                    data.Initialise(tilesPool, false);
                    temporaryArray[x, z] = data;
                }
            }
        }
    }

    public void GenerateTemporaryArrayDestruction(Vector3 initialTile, Vector3 currentTargetTile, ObjectPoolMasterclass tilesPool)
    {
        temporaryArray = CreateNewArray(array);

        if (initialTile.x == currentTargetTile.x && initialTile.z == currentTargetTile.z)
        {
            if(temporaryArray[(int)initialTile.x, (int)initialTile.z] == null)
            {
                GenerationData data = new GenerationData();
                data.Initialise(tilesPool, false);
                temporaryArray[(int)initialTile.x, (int)initialTile.z] = data;
            }
            else
            {
                temporaryArray[(int)initialTile.x, (int)initialTile.z].EnableDestruction(true);
            }
        }
        else if (initialTile.x > currentTargetTile.x && initialTile.z == currentTargetTile.z)
        {
            for (int x = (int)initialTile.x; x > currentTargetTile.x - 1; x--)
            {
                if(temporaryArray[x, (int)currentTargetTile.z] == null)
                {
                    GenerationData data = new GenerationData();
                    data.Initialise(tilesPool, false);
                    temporaryArray[x, (int)currentTargetTile.z] = data;
                }
                else
                {
                    temporaryArray[x, (int)currentTargetTile.z].EnableDestruction(true);
                }
            }
        }
        else if (initialTile.x < currentTargetTile.x && initialTile.z == currentTargetTile.z)
        {
            for (int x = (int)initialTile.x; x < currentTargetTile.x + 1; x++)
            {
                if(temporaryArray[x, (int)currentTargetTile.z] == null)
                {
                    GenerationData data = new GenerationData();
                    data.Initialise(tilesPool, false);
                    temporaryArray[x, (int)currentTargetTile.z] = data;
                }
                else
                {
                    temporaryArray[x, (int)currentTargetTile.z].EnableDestruction(true);
                }
            }
        }
        else if (initialTile.x == currentTargetTile.x && initialTile.z < currentTargetTile.z)
        {
            for (int z = (int)initialTile.z; z < currentTargetTile.z + 1; z++)
            {
                if(temporaryArray[(int)initialTile.x, z] == null)
                {
                    GenerationData data = new GenerationData();
                    data.Initialise(tilesPool, false);
                    temporaryArray[(int)initialTile.x, z] = data;
                }
                else
                {
                    temporaryArray[(int)initialTile.x, z].EnableDestruction(true);
                }
            }
        }
        else if (initialTile.x == currentTargetTile.x && initialTile.z > currentTargetTile.z)
        {
            for (int z = (int)initialTile.z; z > currentTargetTile.z - 1; z--)
            {
                if(temporaryArray[(int)initialTile.x, z] == null)
                {
                    GenerationData data = new GenerationData();
                    data.Initialise(tilesPool, false);
                    temporaryArray[(int)initialTile.x, z] = data;
                }
                else
                {
                    temporaryArray[(int)initialTile.x, z].EnableDestruction(true);
                }
                
            }
        }
        else if (initialTile.x < currentTargetTile.x && initialTile.z < currentTargetTile.z)
        {
            for (int x = (int)initialTile.x; x < currentTargetTile.x + 1; x++)
            {
                for (int z = (int)initialTile.z; z < currentTargetTile.z + 1; z++)
                {
                    if(temporaryArray[x, z] == null)
                    {
                        GenerationData data = new GenerationData();
                        data.Initialise(tilesPool, false);
                        temporaryArray[x, z] = data;
                    }
                    else
                    {
                        temporaryArray[x, z].EnableDestruction(true);
                    }
                }
            }
        }
        else if (initialTile.x > currentTargetTile.x && initialTile.z < currentTargetTile.z)
        {
            for (int x = (int)initialTile.x; x > currentTargetTile.x - 1; x--)
            {
                for (int z = (int)initialTile.z; z < currentTargetTile.z + 1; z++)
                {
                    if(temporaryArray[x, z] == null)
                    {
                        GenerationData data = new GenerationData();
                        data.Initialise(tilesPool, false);
                        temporaryArray[x, z] = data;
                    }
                    else
                    {
                        temporaryArray[x, z].EnableDestruction(true);
                    }
                }
            }
        }
        else if (initialTile.x < currentTargetTile.x && initialTile.z > currentTargetTile.z)
        {
            for (int x = (int)initialTile.x; x < currentTargetTile.x + 1; x++)
            {
                for (int z = (int)initialTile.z; z > currentTargetTile.z - 1; z--)
                {
                    if(temporaryArray[x, z] == null)
                    {
                        GenerationData data = new GenerationData();
                        data.Initialise(tilesPool, false);
                        temporaryArray[x, z] = data;
                    }
                    else
                    {
                        temporaryArray[x, z].EnableDestruction(true);
                    }
                }
            }
        }
        else if (initialTile.x > currentTargetTile.x && initialTile.z > currentTargetTile.z)
        {
            for (int x = (int)initialTile.x; x > currentTargetTile.x - 1; x--)
            {
                for (int z = (int)initialTile.z; z > currentTargetTile.z - 1; z--)
                {
                    if(temporaryArray[x, z] == null)
                    {
                        GenerationData data = new GenerationData();
                        data.Initialise(tilesPool, false);
                        temporaryArray[x, z] = data;
                    }
                    else
                    {
                        temporaryArray[x, z].EnableDestruction(true);
                    }
                }
            }
        }
    }
}





    /*
    void DestroyFromArray(Vector3 initialTile, Vector3 currentTargetTile)
    {
        if (initialTile.x == currentTargetTile.x && initialTile.z == currentTargetTile.z)
        {
            temporaryTilesArray[(int)initialTile.x, (int)initialTile.z] = null;
        }
        else if (initialTile.x > currentTargetTile.x && initialTile.z == currentTargetTile.z)
        {
            for (int x = (int)initialTile.x; x > currentTargetTile.x - 1; x--)
            {
                temporaryTilesArray[x, (int)currentTargetTile.z] = null;
            }
        }
        else if (initialTile.x < currentTargetTile.x && initialTile.z == currentTargetTile.z)
        {
            for (int x = (int)initialTile.x; x < currentTargetTile.x + 1; x++)
            {
                temporaryTilesArray[x, (int)currentTargetTile.z] = null;
            }
        }
        else if (initialTile.x == currentTargetTile.x && initialTile.z < currentTargetTile.z)
        {
            for (int z = (int)initialTile.z; z < currentTargetTile.z + 1; z++)
            {
                temporaryTilesArray[(int)initialTile.x, z] = null;
            }
        }
        else if (initialTile.x == currentTargetTile.x && initialTile.z > currentTargetTile.z)
        {
            for (int z = (int)initialTile.z; z > currentTargetTile.z - 1; z--)
            {
                temporaryTilesArray[(int)initialTile.x, z] = null;
            }
        }
        else if (initialTile.x < currentTargetTile.x && initialTile.z < currentTargetTile.z)
        {
            for (int x = (int)initialTile.x; x < currentTargetTile.x + 1; x++)
            {
                for (int z = (int)initialTile.z; z < currentTargetTile.z + 1; z++)
                {
                    temporaryTilesArray[x, z] = null;
                }
            }
        }
        else if (initialTile.x > currentTargetTile.x && initialTile.z < currentTargetTile.z)
        {
            for (int x = (int)initialTile.x; x > currentTargetTile.x - 1; x--)
            {
                for (int z = (int)initialTile.z; z < currentTargetTile.z + 1; z++)
                {
                    temporaryTilesArray[x, z] = null;
                }
            }
        }
        else if (initialTile.x < currentTargetTile.x && initialTile.z > currentTargetTile.z)
        {
            for (int x = (int)initialTile.x; x < currentTargetTile.x + 1; x++)
            {
                for (int z = (int)initialTile.z; z > currentTargetTile.z - 1; z--)
                {
                    temporaryTilesArray[x, z] = null;
                }
            }
        }
        else if (initialTile.x > currentTargetTile.x && initialTile.z > currentTargetTile.z)
        {
            for (int x = (int)initialTile.x; x > currentTargetTile.x - 1; x--)
            {
                for (int z = (int)initialTile.z; z > currentTargetTile.z - 1; z--)
                {
                    temporaryTilesArray[x, z] = null;
                }
            }
        }
    }
}

    

    */


















/*
public void GenerateContent(Vector3 startTile, Vector3 endTile, ObjectPoolMasterclass tilesPool)
{
  //  ReturnContentToPools();
    GenerateTemporaryArray(startTile, endTile, tilesPool);
   // GenerateTiles();
    //GenerateWalls();
    // GenerateMajorColumns();
    // GenerateMinorColumns();
    // GenerateDoors();
}

public void FinaliseGeneration()
{
    tilesArray = temporaryTilesArray;
}

void ReturnContentToPools()
{
    for(int i = 0; i < contentsList.Count; i++)
    {
        contentsList[i].ReturnChildToPool();
    }

    contentsList.Clear();
}
*/


/*
void GenerateTiles()
{
    for (int x = 0; x < temporaryTilesArray.GetLength(0); x++)
    {
        for (int z = 0; z < temporaryTilesArray.GetLength(1); z++)
        {
            PoolChild newTile = temporaryTilesArray[x, z].RequestObject();
            newTile.transform.position = new Vector3(x, 0, z);
            contentsList.Add(newTile);
        }
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
    calculate.y += 0.5f;

  //  PoolChild poolChild = tilesArray[x, y].RequestWall();
//    poolChild.gameObject.transform.position = calculate;

    calculate = Vector3.zero;
    calculate.y = 90;
 //   poolChild.gameObject.transform.eulerAngles = calculate;
 //   contentsList.Add(poolChild);
}

void GenerateBottomWall(int x, int y)
{
    if (tilesArray[x, y - 1] != null)
        return;

    Vector3 calculate = tilesArray[x, y].transform.position;
    calculate.z -= 0.5f;
    calculate.y += 0.5f;

  //  PoolChild poolChild = tilesArray[x, y].RequestWall();
  //  poolChild.gameObject.transform.position = calculate;

    calculate = Vector3.zero;
    calculate.y = 90;
//    poolChild.gameObject.transform.eulerAngles = calculate;
//    contentsList.Add(poolChild);
}

void GenerateRightWall(int x, int y)
{
    if (tilesArray[x + 1, y] != null)
        return;

    Vector3 calculate = tilesArray[x, y].transform.position;
    calculate.x += 0.5f;
    calculate.y += 0.5f;

   // PoolChild poolChild = tilesArray[x, y].RequestWall();
 //   poolChild.gameObject.transform.position = calculate;

    calculate = Vector3.zero;
  //  poolChild.gameObject.transform.eulerAngles = calculate;
   // contentsList.Add(poolChild);

}

void GenerateLeftWall(int x, int y)
{
    if (tilesArray[x - 1, y] != null)
        return;

    Vector3 calculate = tilesArray[x, y].transform.position;
    calculate.x -= 0.5f;
    calculate.y += 0.5f;

  // PoolChild poolChild = tilesArray[x, y].RequestWall();
 //   poolChild.gameObject.transform.position = calculate;

    calculate = Vector3.zero;
  //  poolChild.gameObject.transform.eulerAngles = calculate;
  //  contentsList.Add(poolChild);
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

*/





/*
public void AssignObjectToArray(TileMasterClass child, int x, int y)
{
    x += 1;
    y += 1;

    // if location already occupied > release location
    if (tilesArray[x, y] != null)
    {
        tilesArray[x, y].ReturnToPool();
    }

    tilesArray[x, y] = child;
}

public void DisableOjectInArray(int x, int y)
{
    x += 1;
    y += 1;

    if (tilesArray[x, y] == null)
        return;

    tilesArray[x, y].gameObject.SetActive(false);

}

public void ActivateObjectInArray(int x, int y)
{
    x += 1;
    y += 1;

    if (tilesArray[x, y] == null)
        return;

    tilesArray[x, y].gameObject.SetActive(true);
}
*/

/*
 * might use later
public void ReleaseObjectFromArray(int x, int y)
{
    x += 1;
    y += 1;

    tilesArray[x, y].ReturnToPool();
    tilesArray[x, y] = null;
}
*/