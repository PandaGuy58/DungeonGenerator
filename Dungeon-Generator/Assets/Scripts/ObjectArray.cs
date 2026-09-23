using System;
using UnityEngine;

// refactor into a struct
public class GenerationData
{
    public Biome biome { get; private set; }
    public bool destruction { get; private set; }

    public void Initialise(Biome biome, bool destruction)
    {
        this.destruction = destruction;
        this.biome = biome;
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
    GenerationData[,] array;
    GenerationData[,] temporaryArray;

    private Action<ExecuteTempArrayGenerationEvent> _onExecuteTempArray;

    private void Awake()
    {
        instance = this;
        array = new GenerationData[51, 51];
    }

    private void OnEnable()
    {
        _onExecuteTempArray = GenerateTemporaryArray;
        EventBus<ExecuteTempArrayGenerationEvent>.Subscribe(_onExecuteTempArray);
    }

    private void OnDisable()
    {
        EventBus<ExecuteTempArrayGenerationEvent>.Unsubscribe(_onExecuteTempArray);
    }

    public GenerationData[,] RequestTemporaryArray()
    {
        return temporaryArray;
    }

    public void FinaliseArray()
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
                data.Initialise(oldArray[x, z].biome, oldArray[x, z].destruction);
                newArray[x, z] = data;
            }
        }

        return newArray;
    }

    void AssignArrayElement(int x, int z, Biome biome)
    {
        GenerationData data = new GenerationData();
        if (biome.IsDestructive())
        {
            if (temporaryArray[x, z] == null)
            {
                data.Initialise(biome, true);
                temporaryArray[x, z] = data;
            }
            else
            {
                temporaryArray[x, z].EnableDestruction(true);
            }
        }
        else
        {
            data.Initialise(biome, false);
            temporaryArray[x, z] = data;
        }
    }

    public void GenerateTemporaryArray(ExecuteTempArrayGenerationEvent evt)
    {
        temporaryArray = CreateNewArray(array);

        if (evt.initialTile.x == evt.currentTargetTile.x && evt.initialTile.z == evt.currentTargetTile.z)
        {
            AssignArrayElement((int)evt.initialTile.x, (int)evt.initialTile.z, evt.biome);
        }
        else if (evt.initialTile.x > evt.currentTargetTile.x && evt.initialTile.z == evt.currentTargetTile.z)
        {
            for (int x = (int)evt.initialTile.x; x > evt.currentTargetTile.x - 1; x--)
            {
                AssignArrayElement(x, (int)evt.currentTargetTile.z, evt.biome);
            }
        }
        else if (evt.initialTile.x < evt.currentTargetTile.x && evt.initialTile.z == evt.currentTargetTile.z)
        {
            for (int x = (int)evt.initialTile.x; x < evt.currentTargetTile.x + 1; x++)
            {
                AssignArrayElement(x, (int)evt.currentTargetTile.z, evt.biome);
            }
        }
        else if (evt.initialTile.x == evt.currentTargetTile.x && evt.initialTile.z < evt.currentTargetTile.z)
        {
            for (int z = (int)evt.initialTile.z; z < evt.currentTargetTile.z + 1; z++)
            {
                AssignArrayElement((int)evt.initialTile.x, z, evt.biome);

            }
        }
        else if (evt.initialTile.x == evt.currentTargetTile.x && evt.initialTile.z > evt.currentTargetTile.z)
        {
            for (int z = (int)evt.initialTile.z; z > evt.currentTargetTile.z - 1; z--)
            {
                AssignArrayElement((int)evt.initialTile.x, z, evt.biome);
            }
        }
        else if (evt.initialTile.x < evt.currentTargetTile.x && evt.initialTile.z < evt.currentTargetTile.z)
        {
            for (int x = (int)evt.initialTile.x; x < evt.currentTargetTile.x + 1; x++)
            {
                for (int z = (int)evt.initialTile.z; z < evt.currentTargetTile.z + 1; z++)
                {
                    AssignArrayElement(x, z, evt.biome);
                }
            }
        }
        else if (evt.initialTile.x > evt.currentTargetTile.x && evt.initialTile.z < evt.currentTargetTile.z)
        {
            for (int x = (int)evt.initialTile.x; x > evt.currentTargetTile.x - 1; x--)
            {
                for (int z = (int)evt.initialTile.z; z < evt.currentTargetTile.z + 1; z++)
                {
                    AssignArrayElement(x, z, evt.biome);
                }
            }
        }
        else if (evt.initialTile.x < evt.currentTargetTile.x && evt.initialTile.z > evt.currentTargetTile.z)
        {
            for (int x = (int)evt.initialTile.x; x < evt.currentTargetTile.x + 1; x++)
            {
                for (int z = (int)evt.initialTile.z; z > evt.currentTargetTile.z - 1; z--)
                {
                    AssignArrayElement(x, z, evt.biome);
                }
            }
        }
        else if (evt.initialTile.x > evt.currentTargetTile.x && evt.initialTile.z > evt.currentTargetTile.z)
        {
            for (int x = (int)evt.initialTile.x; x > evt.currentTargetTile.x - 1; x--)
            {
                for (int z = (int)evt.initialTile.z; z > evt.currentTargetTile.z - 1; z--)
                {
                    AssignArrayElement(x, z, evt.biome);
                }
            }
        }
    }

    public void RemoveFromArray()
    {
        for (int x = 0; x < temporaryArray.GetLength(0); x++)
        {
            for (int y = 0; y < temporaryArray.GetLength(1); y++)
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