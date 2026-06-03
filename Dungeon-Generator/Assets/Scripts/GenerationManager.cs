using NUnit.Framework.Constraints;
using System.Collections.Generic;
using UnityEngine;

public class GenerationManager : MonoBehaviour
{
    public static GenerationManager instance;
    List<PoolChild> generatedTiles = new List<PoolChild>();

    private void Awake()
    {
        instance = this;
    }

    public void Generate()
    {
        GenerationData[,] array = ObjectArray.instance.RequestTemporaryArray();
        ReturnTilesToPool();
        GenerateTiles(array);
    }

    void ReturnTilesToPool()
    {
        for (int i = 0; i < generatedTiles.Count; i++)
        {
            generatedTiles[i].ReturnChildToPool();
        }
        generatedTiles.Clear();
    }

    void GenerateTiles(GenerationData[,] dataArray)
    {
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
}
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