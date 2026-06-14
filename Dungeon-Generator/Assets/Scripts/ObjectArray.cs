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
                if (oldArray[x, z] == null)
                    continue;

                GenerationData data = new GenerationData();
                data.Initialise(oldArray[x, z].pool, oldArray[x, z].destruction);
                newArray[x, z] = data;
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
                data.Initialise(tilesPool, true);
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
                    data.Initialise(tilesPool, true);
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
                    data.Initialise(tilesPool, true);
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
                    data.Initialise(tilesPool, true);
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
                    data.Initialise(tilesPool, true);
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
                        data.Initialise(tilesPool, true);
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
                        data.Initialise(tilesPool, true);
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
                        data.Initialise(tilesPool, true);
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
                        data.Initialise(tilesPool, true);
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

    public void RemoveFromArray()
    {
        for(int x = 0; x < temporaryArray.GetLength(0); x++)
        {
            for(int y = 0;  y < temporaryArray.GetLength(1); y++)
            {
                if (temporaryArray[x, y] == null)
                    continue;

                if (!temporaryArray[x, y].destruction)
                    continue;

                temporaryArray[x, y] = null;
            }
        }
    }
}
