using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    [SerializeField] List<Biome> biomes;
    [SerializeField] int selectedBiome = 0;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {

            selectedBiome++;
            if (selectedBiome > biomes.Count - 1)
            {
                selectedBiome = 0;
            }
        }



        if (Input.GetKeyDown(KeyCode.Space))
        {
            PoolChild newObject = ObjectPool.instance.GetInstance(biomes[selectedBiome].FloorPrefab());
        }
    }
}
