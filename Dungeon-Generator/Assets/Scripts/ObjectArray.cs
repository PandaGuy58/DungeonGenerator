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
    // array is larger than needed by 2 at x and y
    // x.0, y.0, x.Length, y.Length are empty for logic purposes
    private GenerationData[,] array;
    private GenerationData[,] temporaryArray;

    private Action<TempArrayGenerationEvent> onTempArrayGenerationEvent;
    private Action<FinaliseArrayEvent> onFinaliseArrayEvent;
    private Action<RemoveFromTempArrayEvent> onRemoveFromTempArrayEvent;

    private void Awake()
    {
        array = new GenerationData[51, 51];
    }

    private void OnEnable()
    {
        onTempArrayGenerationEvent = GenerateTemporaryArray;
        onFinaliseArrayEvent = FinaliseArray;
        onRemoveFromTempArrayEvent = RemoveFromTempArray;
        EventBus<TempArrayGenerationEvent>.Subscribe(onTempArrayGenerationEvent);
        EventBus<FinaliseArrayEvent>.Subscribe(onFinaliseArrayEvent);
        EventBus<RemoveFromTempArrayEvent>.Subscribe(RemoveFromTempArray);
    }

    private void OnDisable()
    {
        EventBus<TempArrayGenerationEvent>.Unsubscribe(onTempArrayGenerationEvent);
        EventBus<FinaliseArrayEvent>.Unsubscribe(onFinaliseArrayEvent);
        EventBus<RemoveFromTempArrayEvent>.Unsubscribe(RemoveFromTempArray);
    }

    public GenerationData[,] RequestTemporaryArray()
    {
        return temporaryArray;
    }

    public void FinaliseArray(FinaliseArrayEvent evt)
    {
        array = temporaryArray;

        EventBus<GenerateContentsEvent>.Publish(new GenerateContentsEvent
        {
            GenerationDataArray = temporaryArray
        });
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

    public void GenerateTemporaryArray(TempArrayGenerationEvent evt)
    {
        temporaryArray = CreateNewArray(array);

        if (evt.InitialTile.x == evt.CurrentTargetTile.x && evt.InitialTile.z == evt.CurrentTargetTile.z)
        {
            AssignArrayElement((int)evt.InitialTile.x, (int)evt.InitialTile.z, evt.Biome);
        }
        else if (evt.InitialTile.x > evt.CurrentTargetTile.x && evt.InitialTile.z == evt.CurrentTargetTile.z)
        {
            for (int x = (int)evt.InitialTile.x; x > evt.CurrentTargetTile.x - 1; x--)
            {
                AssignArrayElement(x, (int)evt.CurrentTargetTile.z, evt.Biome);
            }
        }
        else if (evt.InitialTile.x < evt.CurrentTargetTile.x && evt.InitialTile.z == evt.CurrentTargetTile.z)
        {
            for (int x = (int)evt.InitialTile.x; x < evt.CurrentTargetTile.x + 1; x++)
            {
                AssignArrayElement(x, (int)evt.CurrentTargetTile.z, evt.Biome);
            }
        }
        else if (evt.InitialTile.x == evt.CurrentTargetTile.x && evt.InitialTile.z < evt.CurrentTargetTile.z)
        {
            for (int z = (int)evt.InitialTile.z; z < evt.CurrentTargetTile.z + 1; z++)
            {
                AssignArrayElement((int)evt.InitialTile.x, z, evt.Biome);

            }
        }
        else if (evt.InitialTile.x == evt.CurrentTargetTile.x && evt.InitialTile.z > evt.CurrentTargetTile.z)
        {
            for (int z = (int)evt.InitialTile.z; z > evt.CurrentTargetTile.z - 1; z--)
            {
                AssignArrayElement((int)evt.InitialTile.x, z, evt.Biome);
            }
        }
        else if (evt.InitialTile.x < evt.CurrentTargetTile.x && evt.InitialTile.z < evt.CurrentTargetTile.z)
        {
            for (int x = (int)evt.InitialTile.x; x < evt.CurrentTargetTile.x + 1; x++)
            {
                for (int z = (int)evt.InitialTile.z; z < evt.CurrentTargetTile.z + 1; z++)
                {
                    AssignArrayElement(x, z, evt.Biome);
                }
            }
        }
        else if (evt.InitialTile.x > evt.CurrentTargetTile.x && evt.InitialTile.z < evt.CurrentTargetTile.z)
        {
            for (int x = (int)evt.InitialTile.x; x > evt.CurrentTargetTile.x - 1; x--)
            {
                for (int z = (int)evt.InitialTile.z; z < evt.CurrentTargetTile.z + 1; z++)
                {
                    AssignArrayElement(x, z, evt.Biome);
                }
            }
        }
        else if (evt.InitialTile.x < evt.CurrentTargetTile.x && evt.InitialTile.z > evt.CurrentTargetTile.z)
        {
            for (int x = (int)evt.InitialTile.x; x < evt.CurrentTargetTile.x + 1; x++)
            {
                for (int z = (int)evt.InitialTile.z; z > evt.CurrentTargetTile.z - 1; z--)
                {
                    AssignArrayElement(x, z, evt.Biome);
                }
            }
        }
        else if (evt.InitialTile.x > evt.CurrentTargetTile.x && evt.InitialTile.z > evt.CurrentTargetTile.z)
        {
            for (int x = (int)evt.InitialTile.x; x > evt.CurrentTargetTile.x - 1; x--)
            {
                for (int z = (int)evt.InitialTile.z; z > evt.CurrentTargetTile.z - 1; z--)
                {
                    AssignArrayElement(x, z, evt.Biome);
                }
            }
        }

        EventBus<GenerateTilesEvent>.Publish(new GenerateTilesEvent
        {
            GenerationDataArray = temporaryArray
        });
    }

    public void RemoveFromTempArray(RemoveFromTempArrayEvent evt)
    {
        for (int x = 0; x < temporaryArray.GetLength(0); x++)
        {
            for (int y = 0; y < temporaryArray.GetLength(1); y++)
            {
                if (temporaryArray[x, y] == null)
                    continue;

                if (temporaryArray[x, y].destruction)
                    temporaryArray[x, y] = null;
            }
        }

        EventBus<GenerateTilesEvent>.Publish(new GenerateTilesEvent
        {
            GenerationDataArray = temporaryArray
        });

    }
}