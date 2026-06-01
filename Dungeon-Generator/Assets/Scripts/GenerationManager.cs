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
        ObjectPoolMasterclass[,] tilesArray = ObjectArray.instance.RequestTemporaryTilesArray();
        ReturnTilesToPool();
        GenerateTiles(tilesArray);
    }

    void ReturnTilesToPool()
    {
        for (int i = 0; i < generatedTiles.Count; i++)
        {
            generatedTiles[i].ReturnChildToPool();
        }
        generatedTiles.Clear();
    }

    void GenerateTiles(ObjectPoolMasterclass[,] tilesArray)
    {
        for (int x = 0; x < tilesArray.GetLength(0); x++)
        {
            for (int z = 0; z < tilesArray.GetLength(1); z++)
            {
                if (tilesArray[x, z] == null)
                    continue;

                PoolChild newTile = tilesArray[x, z].RequestObject();
                newTile.transform.position = new Vector3(x, 0, z);
                generatedTiles.Add(newTile);
            }
        }
    }
}
