using System.Collections.Generic;
using UnityEngine;

public class GenerationManager : MonoBehaviour
{
    public static GenerationManager instance;
    List<PoolChild> tiles = new List<PoolChild>();
    List<PoolChild> contents = new List<PoolChild>();

    private void Awake()
    {
        instance = this;
    }

    void DestroyObjects(List<PoolChild> objects)
    {
        Debug.Log(Time.time);
        for (int i = 0; i < objects.Count; i++)
        {
            ObjectPool.instance.ReturnInstance(objects[i]);
        }
        objects.Clear();

    }

    public void DestroyContents()
    {
        for (int i = 0; i < contents.Count; i++)
        {
            ObjectPool.instance.ReturnInstance(contents[i]);
        }
        contents.Clear();
    }

    void PlaceObjectRotate(GameObject prefab, Vector3 position, Vector3 rotation, int x, int y)
    {
        PoolChild instance = ObjectPool.instance.GetInstance(prefab);
        Vector3 calculate = new Vector3(x + 0.5f, 0.1f, y - 0.5f);
        calculate += position;
        //   GameObject newObject = Instantiate(prefab, calculate, Quaternion.identity);
        instance.transform.position = calculate;
        instance.transform.eulerAngles = rotation;
        contents.Add(instance);
    }

    PoolChild PlaceObject(GameObject prefab, Vector3 position, int x, int y, bool addToContents)
    {
        PoolChild instance = ObjectPool.instance.GetInstance(prefab);
        Vector3 calculate = new Vector3(x + 0.5f, 0.1f, y - 0.5f);
        calculate += position;        
        instance.transform.position = calculate;

        if (addToContents)
        {
            contents.Add(instance);
        }

        return instance;        
    }

    public void RegenerateTiles()
    {
        GenerationData[,] array = ObjectArray.instance.RequestTemporaryArray();
        DestroyObjects(tiles);
        for (int x = 0; x < array.GetLength(0); x++)
        {
            for (int z = 0; z < array.GetLength(1); z++)
            {
                Tile(x, z, array);
            }
        }
    }

    void Tile(int x, int z, GenerationData[,] array)
    {
        if (array[x, z] == null)
            return;

        Vector3 coordinate = new Vector3(x + 0.5f, 0.1f, z - 0.5f);
        GameObject prefab = array[x, z].biome.FloorPrefab();
        PoolChild newTile = PlaceObject(prefab, Vector3.zero, x, z, false);
        tiles.Add(newTile);

        if (array[x, z].biome.IsDestructive())
            return;

        ControlShader controlShader = newTile.GetComponent<ControlShader>();

        if (array[x, z].destruction)
        {
            controlShader.Activate(true);
        }
        else
        {
            controlShader.Activate(false);
        }
    }

    public void GenerateContents()
    {
        GenerationData[,] array = ObjectArray.instance.RequestTemporaryArray();

        for (int x = 0; x < array.GetLength(0); x++)
        {
            for (int z = 0; z < array.GetLength(1); z++)
            {
                if (array[x, z] == null)
                    continue;

                GenerateWalls(x, z, array);
                GenerateOutsideCorners(x, z, array);
                GenerateInsideCorners(x, z, array);
                GenerateWallSplits(x, z, array);
                GenerateColumns(x, z, array); 
            }
        }
    }

    bool CheckWall(int x, int y, GenerationData[,] array, Biome biome)
    {
        if (array[x, y] == null)
            return true;

        if (array[x, y].biome == biome)
            return false;

        if (biome.StopWallGeneration())
            return false;

        if (array[x, y].biome.StopWallGeneration())
            return false;

        return true;
    }

    bool CheckTopWall(int x, int y, GenerationData[,] array)
    {
        if (array[x, y] == null)
            return false;

        return CheckWall(x, y + 1, array, array[x, y].biome);
    }

    bool CheckBottomWall(int x, int y, GenerationData[,] array)
    {
        if (array[x, y] == null)
            return false;

        return CheckWall(x, y - 1, array, array[x, y].biome);
    }

    bool CheckLeftWall(int x, int y, GenerationData[,] array)
    {
        if (array[x, y] == null)
            return false;

        return CheckWall(x - 1, y, array, array[x, y].biome);
    }

    bool CheckRightWall(int x, int y, GenerationData[,] array)
    {
        if (array[x, y] == null)
            return false;

        return CheckWall(x + 1, y, array, array[x, y].biome);
    }

    bool CheckSameBiome(int x, int y, GenerationData[,] array, Biome biome)
    {
        if (array[x, y] == null)
            return false;

        if (array[x, y].biome == biome)
            return true;

        return false;
    }

    void GenerateWalls(int x, int y, GenerationData[,] array)
    {
        if (CheckTopWall(x, y, array))
        {
            TopWall(x, y, array);
        }

        if (CheckBottomWall(x, y, array))
        {
            BottomWall(x, y, array);
        }

        if (CheckLeftWall(x, y, array))
        {
            LeftWall(x, y, array);
        }

        if (CheckRightWall(x, y, array))
        {
            RightWall(x, y, array);
        }
    }

    void TopWall(int x, int y, GenerationData[,] array)
    {
        GameObject prefab = array[x, y].biome.WallPrefab();
        PlaceObjectRotate(prefab, Vector3.zero, Vector3.zero, x, y);
    }


    void BottomWall(int x, int y, GenerationData[,] array)
    {
        GameObject prefab = array[x, y].biome.WallPrefab();
        Vector3 position = new Vector3(-1, 0, +1);
        Vector3 rotation = new Vector3(0, 180, 0);
        PlaceObjectRotate(prefab, position, rotation, x, y);

    }


    void RightWall(int x, int y, GenerationData[,] array)
    {
        GameObject prefab = array[x, y].biome.WallPrefab();
        Vector3 position = new Vector3(-1, 0, 0);
        Vector3 rotation = new Vector3(0, 90, 0);
        PlaceObjectRotate(prefab, position, rotation, x, y);
    }

    void LeftWall(int x, int y, GenerationData[,] array)
    {
        GameObject prefab = array[x, y].biome.WallPrefab();
        Vector3 position = new Vector3(0, 0, 1);
        Vector3 rotation = new Vector3(0, -90, 0);
        PlaceObjectRotate(prefab, position, rotation, x, y);
    }

    void GenerateOutsideCorners(int x, int y, GenerationData[,] array)
    {
        TopLeftOutsideCorner(x, y, array);
        TopRightOutsideCorner(x, y, array);
        BottomLeftOutsideCorner(x, y, array);
        BottomRightOutsideCorner(x, y, array);
    }

    void TopLeftOutsideCorner(int x, int y, GenerationData[,] array)
    {
        bool tileTransitionOne = false;

        if (!CheckTopWall(x, y, array))
        {
            tileTransitionOne = true;
        }

        bool tileTransitionTwo = false;

        if (!CheckLeftWall(x, y, array))
        {
            tileTransitionTwo = true;
        }

        Vector3 position;

        if (tileTransitionOne && tileTransitionTwo)
        {
            if (array[x - 1, y + 1] == null)
                return;

            if (!array[x, y].biome.StopWallGeneration())
                return;

            if (array[x, y].biome == array[x - 1, y].biome)
                return;

            if (array[x, y].biome == array[x, y + 1].biome)
                return;

            position = new Vector3(-1, 0.5f, 1);
        }
        else if(!tileTransitionOne && !tileTransitionTwo)
        {
            position = new Vector3(-0.85f, 0.5f, 0.85f);
        }
        else
        {
            return;
        }

        GameObject prefab = array[x, y].biome.BigColumnPrefab();
        PlaceObject(prefab, position, x, y, true);
    }

    void TopRightOutsideCorner(int x, int y, GenerationData[,] array)
    {
        bool tileTransitionOne = false;

        if (!CheckTopWall(x, y, array))
        {
            tileTransitionOne = true;
        }

        bool tileTransitionTwo = false;

        if (!CheckRightWall(x, y, array))
        {
            tileTransitionTwo = true;
        }

        Vector3 position;

        if(tileTransitionOne && tileTransitionTwo)
        {
            if (array[x + 1, y + 1] == null)
                return;

            if (!array[x, y].biome.StopWallGeneration())
                return;

            if (array[x, y].biome == array[x + 1, y].biome)
                return;

            if (array[x, y].biome == array[x, y + 1].biome)
                return;

            position = new Vector3(0, 0.5f, 1);
        }
        else if(!tileTransitionOne && !tileTransitionTwo)
        {
            position = new Vector3(-0.15f, 0.5f, 0.85f);
        }
        else
        {
            return;
        }

        GameObject prefab = array[x, y].biome.BigColumnPrefab();
        PlaceObject(prefab, position, x, y, true);

    }

    void BottomLeftOutsideCorner(int x, int y, GenerationData[,] array)
    {
        bool tileTransitionOne = false;

        if (!CheckBottomWall(x, y, array))
        {
            tileTransitionOne = true;
        }

        bool tileTransitionTwo = false;

        if (!CheckLeftWall(x, y, array))
        {
            tileTransitionTwo = true;
        }

        Vector3 position;

        if (tileTransitionOne && tileTransitionTwo)
        {
            if (array[x - 1, y - 1] == null)
                return;

            if (!array[x, y].biome.StopWallGeneration())
                return;

            if (array[x, y].biome == array[x - 1, y].biome)
                return;

            if (array[x, y].biome == array[x, y - 1].biome)
                return;

            position = new Vector3(-1, 0.5f, 0);
        }
        else if(!tileTransitionOne && !tileTransitionTwo)
        {
            position = new Vector3(-0.85f, 0.5f, 0.15f);
        }
        else
        {
            return;
        }

        GameObject prefab = array[x, y].biome.BigColumnPrefab();
         
        PlaceObject(prefab, position, x, y, true);
    }

    void BottomRightOutsideCorner(int x, int y, GenerationData[,] array)
    {
        bool tileTransitionOne = false;

        if (!CheckBottomWall(x, y, array))
        {
            tileTransitionOne = true;
        }

        bool tileTransitionTwo = false;

        if (!CheckRightWall(x, y, array))
        {
            tileTransitionTwo = true;
        }

        Vector3 position;

        if (tileTransitionOne && tileTransitionTwo)
        {
            if (array[x + 1, y - 1] == null)
                return;

            if (!array[x, y].biome.StopWallGeneration())
                return;

            if (array[x, y].biome == array[x + 1, y].biome)
                return;

            if (array[x, y].biome == array[x, y - 1].biome)
                return;

            position = new Vector3(0, 0.5f, 0);
        }
        else if(!tileTransitionOne && !tileTransitionTwo)
        {
            position = new Vector3(-0.15f, 0.5f, 0.15f);
        }
        else
        {
            return;
        }

        GameObject prefab = array[x, y].biome.BigColumnPrefab();
        PlaceObject(prefab, position, x, y, true);
    }

    void GenerateInsideCorners(int x, int y, GenerationData[,] array)
    {
        TopLeftInsideCorner(x, y, array);
        TopRightInsideCorner(x, y, array);
        BottomLeftInsideCorner(x, y, array);
        BottomRightInsideCorner(x, y, array);
    }

    void TopLeftInsideCorner(int x, int y, GenerationData[,] array)
    {       
        if (CheckLeftWall(x, y, array))
            return;

        if (CheckTopWall(x, y, array))
            return;

        bool tileTransitionOne = false;

        if (!CheckTopWall(x - 1, y, array))
        {                    
            tileTransitionOne = true;
        }

        bool tileTransitionTwo = false;

        if (!CheckLeftWall(x, y + 1, array))
        {
            tileTransitionTwo = true;
        }

        Vector3 position;
        GameObject prefab;

        if (tileTransitionOne && tileTransitionTwo)
        {
            if (!array[x, y].biome.StopWallGeneration())
                return;

            if (array[x, y].biome != array[x - 1, y].biome)
                return;

            if (array[x, y].biome != array[x, y + 1].biome)
                return;

            if (array[x, y].biome == array[x - 1, y + 1].biome)
                return;

            position = new Vector3(-1, 0.5f, 1);
            prefab = array[x, y].biome.BigColumnPrefab();
        }
        else if (!tileTransitionOne && !tileTransitionTwo)
        {
            position = new Vector3(-0.925f, 0.5f, 0.925f);

            if (array[x - 1, y].biome.StopWallGeneration())
            {
                prefab = array[x - 1, y].biome.BigColumnPrefab();
            }
            else if (array[x, y +1].biome.StopWallGeneration())
            {
                prefab = array[x, y + 1].biome.BigColumnPrefab();
            }
            else
            {
                prefab = array[x, y].biome.BigColumnPrefab();
            }
        }
        else
        {
            return;
        }           

        
        PlaceObject(prefab, position, x, y, true);
    }

    void TopRightInsideCorner(int x, int y, GenerationData[,] array)
    {
        if (CheckRightWall(x, y, array))
            return;

        if (CheckTopWall(x, y, array))
            return;

        bool tileTransitionOne = false;

        if (!CheckRightWall(x, y + 1, array))
        {
            tileTransitionOne = true;
        }

        bool tileTransitionTwo = false;

        if (!CheckTopWall(x + 1, y, array))
        {
            tileTransitionTwo = true;
        }

        Vector3 position;
        GameObject prefab;

        if(tileTransitionOne && tileTransitionTwo)
        {
            if (!array[x, y].biome.StopWallGeneration())
                return;

            if (array[x, y].biome != array[x + 1, y].biome)
                return;

            if (array[x, y].biome != array[x, y + 1].biome)
                return;

            if (array[x, y].biome == array[x + 1, y + 1].biome)
                return;

            position = new Vector3(0, 0.5f, 1);
            prefab = array[x, y].biome.BigColumnPrefab();
        }
        else if(!tileTransitionOne && !tileTransitionTwo)
        {
            position = new Vector3(-0.075f, 0.5f, 0.925f);

            if (array[x + 1, y].biome.StopWallGeneration())
            {
                prefab = array[x + 1, y].biome.BigColumnPrefab();
            }
            else if (array[x, y + 1].biome.StopWallGeneration())
            {
                prefab = array[x, y + 1].biome.BigColumnPrefab();
            }
            else
            {
                prefab = array[x, y].biome.BigColumnPrefab();
            }
        }
        else
        {
            return;
        }

        PlaceObject(prefab, position, x, y, true);
    }

    void BottomLeftInsideCorner(int x, int y, GenerationData[,] array)
    {
        if (CheckBottomWall(x, y, array))
            return;

        if (CheckLeftWall(x, y, array))
            return;

        bool tileTransitionOne = false;

        if (!CheckBottomWall(x - 1, y, array))
        {
            tileTransitionOne = true;
        }

        bool tileTransitionTwo = false;

        if (!CheckLeftWall(x, y - 1, array))
        {
            tileTransitionTwo = true;
        }

        Vector3 position;
        GameObject prefab;

        if(tileTransitionOne && tileTransitionTwo)
        {
            if (!array[x, y].biome.StopWallGeneration())
                return;

            if (array[x, y].biome != array[x - 1, y].biome)
                return;

            if (array[x, y].biome != array[x, y - 1].biome)
                return;

            if (array[x, y].biome == array[x - 1, y - 1].biome)
                return;

            position = new Vector3(-1, 0.5f, 0);
            prefab = array[x, y].biome.BigColumnPrefab();
        }
        else if(!tileTransitionOne && !tileTransitionTwo)
        {
            position = new Vector3(-0.925f, 0.5f, 0.075f);

            if (array[x - 1, y].biome.StopWallGeneration())
            {
                prefab = array[x - 1, y].biome.BigColumnPrefab();
            }
            else if (array[x, y - 1].biome.StopWallGeneration())
            {
                prefab = array[x, y - 1].biome.BigColumnPrefab();
            }
            else
            {
                prefab = array[x, y].biome.BigColumnPrefab();
            }
        }
        else
        {
            return;
        }

        PlaceObject(prefab, position, x, y, true);
    }

    void BottomRightInsideCorner(int x, int y, GenerationData[,] array)
    {
        if (CheckRightWall(x, y, array))
            return;

        if (CheckBottomWall(x, y, array))
            return;

        bool tileTransitionOne = false;

        if (!CheckBottomWall(x + 1, y, array))
        {
            tileTransitionOne = true;
        }

        bool tileTransitionTwo = false;

        if (!CheckRightWall(x, y - 1, array))
        {
            tileTransitionTwo = true;
        }

        Vector3 position;
        GameObject prefab;

        if(tileTransitionOne && tileTransitionTwo)
        {
            if (!array[x, y].biome.StopWallGeneration())
                return;

            if (array[x, y].biome != array[x + 1, y].biome)
                return;

            if (array[x, y].biome != array[x, y - 1].biome)
                return;

            if (array[x, y].biome == array[x + 1, y - 1].biome)
                return;

            position = new Vector3(0, 0.5f, 0);
            prefab = array[x, y].biome.BigColumnPrefab();
        }
        else if(!tileTransitionOne && !tileTransitionTwo)
        {
            if (array[x + 1, y].biome.StopWallGeneration())
            {
                prefab = array[x + 1, y].biome.BigColumnPrefab();
            }
            else if (array[x, y - 1].biome.StopWallGeneration())
            {
                prefab = array[x, y - 1].biome.BigColumnPrefab();
            }
            else
            {
                prefab = array[x, y].biome.BigColumnPrefab();
            }

            position = new Vector3(-0.075f, 0.5f, 0.075f);
        }
        else
        {
            return;
        }

        PlaceObject(prefab, position, x, y, true);
    }

    void GenerateWallSplits(int x, int y, GenerationData[,] array)
    {
        if (!array[x, y].biome.StopWallGeneration())
            return;

        NorthRightWallSplit(x, y, array);
        NorthLeftWallSplit(x, y, array);
        SouthRightWallSplit(x, y, array);
        SouthLeftWallSplit(x, y, array);
        EastTopWallSplit(x, y, array);
        EastBottomWallSplit(x, y, array);
        WestTopWallSplit(x, y, array);
        WestBottomWallSplit(x, y, array);
    }

    void NorthRightWallSplit(int x, int y, GenerationData[,] array)
    {
        Vector3 position;

        if (!CheckRightWall(x, y + 1, array))
            return;

        if (CheckRightWall(x, y, array))
        {
            if (array[x, y].biome == array[x, y + 1].biome)
                return;

            position = new Vector3(-0.15f, 0.5f, 1);
        }
        else
        {
            if (array[x + 1, y] == null)
                return;

            if (array[x + 1, y + 1] == null)
                return;

            position = new Vector3(0, 0.5f, 1);            
        }

        GameObject prefab = array[x, y].biome.BigColumnPrefab();
        PlaceObject(prefab, position, x, y, true);
    }

    void NorthLeftWallSplit(int x, int y, GenerationData[,] array)
    {
        Vector3 position;

        if (!CheckLeftWall(x, y + 1, array))
            return;

        if (CheckLeftWall(x, y, array))
        {
            if (array[x, y].biome == array[x, y + 1].biome)
                return;

            position = new Vector3(-0.85f, 0.5f, 1);
        }
        else
        {
            if (array[x - 1, y] == null)
                return;

            if (array[x - 1, y + 1] == null)
                return;

            position = new Vector3(-1, 0.5f, 1f);
        }

        GameObject prefab = array[x, y].biome.BigColumnPrefab();        
        PlaceObject(prefab, position, x, y, true);
    }

    void SouthRightWallSplit(int x, int y, GenerationData[,] array)
    {
        Vector3 position;

        if (!CheckRightWall(x, y - 1, array))
            return;

        if (CheckRightWall(x, y, array))
        {
            if (array[x, y].biome == array[x, y - 1].biome)
                return;

            position = new Vector3(-0.15f, 0.5f, 0);
        }
        else
        {
            if (array[x + 1, y] == null)
                return;

            if (array[x + 1, y - 1] == null)
                return;

            position = new Vector3(0, 0.5f, 0);
        }

        GameObject prefab = array[x, y].biome.BigColumnPrefab();        
        PlaceObject(prefab, position, x, y, true);
    }

    void SouthLeftWallSplit(int x, int y, GenerationData[,] array)
    {
        Vector3 position;

        if (!CheckLeftWall(x, y - 1, array))
            return;

        if (CheckLeftWall(x, y, array))
        {
            if (array[x, y].biome == array[x, y - 1].biome)
                return;

            position = new Vector3(-0.85f, 0.5f, 0);
        }
        else
        {
            if (array[x - 1, y] == null)
                return;

            if (array[x - 1, y - 1] == null)
                return;

            position = new Vector3(-1, 0.5f, 0);
        }

        GameObject prefab = array[x, y].biome.BigColumnPrefab();
        PlaceObject(prefab, position, x, y, true);
    }

    void EastTopWallSplit(int x, int y, GenerationData[,] array)
    {
        Vector3 position;

        if (!CheckTopWall(x + 1, y, array))
            return;

        if (CheckTopWall(x, y, array))
        {
            if (array[x, y].biome == array[x + 1, y].biome)
                return;

            position = new Vector3(0, 0.5f, 0.85f);
        }
        else
        {
            if (array[x, y + 1] == null)
                return;

            if (array[x + 1, y + 1] == null)
                return;

            position = new Vector3(0, 0.5f, 1);
        }

        GameObject prefab = array[x, y].biome.BigColumnPrefab();
        PlaceObject(prefab, position, x, y, true);
    }

    void EastBottomWallSplit(int x, int y, GenerationData[,] array)
    {
        Vector3 position;

        if (!CheckBottomWall(x + 1, y, array))
            return;

        if (CheckBottomWall(x, y, array))
        {
            if (array[x, y].biome == array[x + 1, y].biome)
                return;

            position = new Vector3(0, 0.5f, 0.15f);
        }
        else
        {
            if (array[x, y - 1] == null)
                return;

            if (array[x + 1, y - 1] == null)
                return;

            position = new Vector3(0, 0.5f, 0);
        }

        GameObject prefab = array[x, y].biome.BigColumnPrefab();
        PlaceObject(prefab, position, x, y, true);
    }



    void WestTopWallSplit(int x, int y, GenerationData[,] array)
    {
        Vector3 position;

        if (!CheckTopWall(x - 1, y, array))
            return;

        if (CheckTopWall(x, y, array))
        {
            if (array[x, y].biome == array[x - 1, y].biome)
                return;

            position = new Vector3(-1, 0.5f, 0.85f);
        }
        else
        {
            if (array[x, y + 1] == null)
                return;

            if (array[x - 1, y + 1] == null)
                return;

            position = new Vector3(-1f, 0.5f, 1f);
        }

        GameObject prefab = array[x, y].biome.BigColumnPrefab();
        PlaceObject(prefab, position, x, y, true);
    }

    void WestBottomWallSplit(int x, int y, GenerationData[,] array)
    {
        Vector3 position;

        if (!CheckBottomWall(x - 1, y, array))
            return;

        if (CheckBottomWall(x, y, array))
        {
            if (array[x, y].biome == array[x - 1, y].biome)
                return;

            position = new Vector3(-1, 0.5f, 0.15f);
        }
        else
        {
            if (array[x, y - 1] == null)
                return;

            if (array[x - 1, y - 1] == null)
                return;

            position = new Vector3(-1f, 0.5f, 0);
        }

        GameObject prefab = array[x, y].biome.BigColumnPrefab();
        PlaceObject(prefab, position, x, y, true);
    }

    void GenerateColumns(int x, int y, GenerationData[,] array)
    {
        TopLeftColumn(x, y, array);
        TopRightColumn(x, y, array);
        BottomRightColumn(x, y, array);
        BottomLeftColumn(x, y, array);
    }

    void TopLeftColumn(int x, int y, GenerationData[,] array)
    {
        if (!CheckSameBiome(x - 1, y, array, array[x, y].biome))
            return;

        Vector3 position;

        if (CheckTopWall(x, y, array))
        {
            if (!CheckTopWall(x - 1, y, array))
                return;

            position = new Vector3(-1, 0.5f, 0.85f);
        }
        else if (array[x, y].biome.StopWallGeneration())
        {
            if (CheckSameBiome(x, y + 1, array, array[x, y].biome))
                return;

            if (CheckSameBiome(x - 1, y + 1, array, array[x, y].biome))
                return;

            position = new Vector3(-1, 0.5f, 1f);
        }
        else
        {
            return;
        }

        GameObject prefab = array[x, y].biome.SmallColumnPrefab();
        PlaceObject(prefab, position, x, y, true);
    }

    void TopRightColumn(int x, int y, GenerationData[,] array)
    {
        if (!CheckSameBiome(x, y + 1, array, array[x, y].biome))
            return;

        Vector3 position;

        if (CheckRightWall(x, y, array))
        {
            position = new Vector3(-0.15f, 0.5f, 1);

            if (!CheckRightWall(x, y + 1, array))
                return;
        }
        else if (array[x, y].biome.StopWallGeneration())
        {
            if (CheckSameBiome(x + 1, y, array, array[x, y].biome))
                return;

            if (CheckSameBiome(x + 1, y + 1, array, array[x, y].biome))
                return;

            position = new Vector3(0, 0.5f, 1);
        }
        else
        {
            return;
        }

        GameObject prefab = array[x, y].biome.SmallColumnPrefab();
        PlaceObject(prefab, position, x, y, true);
    }

    void BottomRightColumn(int x, int y, GenerationData[,] array)
    {
        if (!CheckSameBiome(x + 1, y, array, array[x, y].biome))
            return;

        Vector3 position;

        if (CheckBottomWall(x, y, array))
        {
            if (!CheckBottomWall(x + 1, y, array))
                return;

            position = new Vector3(0, 0.5f, 0.15f);
        }
        else if (array[x, y].biome.StopWallGeneration())
        {
            if (CheckSameBiome(x, y - 1, array, array[x, y].biome))
                return;

            if (CheckSameBiome(x + 1, y - 1, array, array[x, y].biome))
                return;

            position = new Vector3(0, 0.5f, 0);
        }
        else
        {
            return;
        }

        GameObject prefab = array[x, y].biome.SmallColumnPrefab();
        PlaceObject(prefab, position, x, y, true);
    }

    void BottomLeftColumn(int x, int y, GenerationData[,] array)
    {
        if (!CheckSameBiome(x, y - 1, array, array[x, y].biome))
            return;

        Vector3 position;

        if(CheckLeftWall(x, y, array))
        {
            if (!CheckLeftWall(x, y -1, array))
                return;

            position = new Vector3(-0.85f, 0.5f, 0);
        }
        else if (array[x,y].biome.StopWallGeneration())
        {
            if (CheckSameBiome(x - 1, y, array, array[x, y].biome))
                return;

            if (CheckSameBiome(x - 1, y - 1, array, array[x, y].biome))
                return;

            position = new Vector3(-1, 0.5f, 0);
        }
        else 
        { 
            return; 
        }

        GameObject prefab = array[x, y].biome.SmallColumnPrefab();
        PlaceObject(prefab, position, x, y, true);
    }
}
