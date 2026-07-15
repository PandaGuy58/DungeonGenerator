using System.Collections.Generic;
using UnityEngine;

public class GenerationManager : MonoBehaviour
{
    public static GenerationManager instance;
    List<GameObject> generatedTiles = new List<GameObject>();
    List<GameObject> contents = new List<GameObject>();

    private void Awake()
    {
        instance = this;
    }

    void DestroyObjects(List<GameObject> objects)
    {
        for (int i = 0; i < objects.Count; i++)
        {
            Destroy(objects[i]);
        }
        objects.Clear();
    }

    public void DestroyContents()
    {
        for (int i = 0; i < contents.Count; i++)
        {
            Destroy(contents[i]);
        }
    }

    void PlaceObjectRotate(GameObject prefab, Vector3 position, Vector3 rotation, int x, int y)
    {
        Vector3 calculate = new Vector3(x + 0.5f, 0.1f, y - 0.5f);
        calculate += position;
        GameObject newObject = Instantiate(prefab, calculate, Quaternion.identity);
        newObject.transform.eulerAngles = rotation;
        contents.Add(newObject);
    }

    void PlaceObject(GameObject prefab, Vector3 position, int x, int y)
    {
        Vector3 calculate = new Vector3(x + 0.5f, 0.1f, y - 0.5f);
        calculate += position;
        GameObject newObject = Instantiate(prefab, calculate, Quaternion.identity);
        contents.Add(newObject);
    }

    public void RegenerateTiles()
    {
        GenerationData[,] array = ObjectArray.instance.RequestTemporaryArray();
        DestroyObjects(generatedTiles);
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
        GameObject newTile = Instantiate(array[x, z].biome.FloorPrefab(), coordinate, Quaternion.identity);
        generatedTiles.Add(newTile);

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
        PlaceObject(prefab, Vector3.zero, x, y);
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
        PlaceObject(prefab, position, x, y);
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
        PlaceObject(prefab, position, x, y);

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
         
        PlaceObject(prefab, position, x, y);
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
        PlaceObject(prefab, position, x, y);
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

        
        PlaceObject(prefab, position, x, y);
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

        PlaceObject(prefab, position, x, y);
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

        PlaceObject(prefab, position, x, y);
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

        PlaceObject(prefab, position, x, y);
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
        PlaceObject(prefab, position, x, y);
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
        PlaceObject(prefab, position, x, y);
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
        PlaceObject(prefab, position, x, y);
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
        PlaceObject(prefab, position, x, y);
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
        PlaceObject(prefab, position, x, y);
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
        PlaceObject(prefab, position, x, y);
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
        PlaceObject(prefab, position, x, y);
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
        PlaceObject(prefab, position, x, y);
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
        PlaceObject(prefab, position, x, y);
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
        PlaceObject(prefab, position, x, y);
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
        PlaceObject(prefab, position, x, y);
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
        PlaceObject(prefab, position, x, y);
    }
}



//Vector3 position = 







/*
 *    void TopLeftWallSplit(int x, int y)
    {
        if (tileArray[x - 1, y] == null)
            return;

        if (tileArray[x - 1, y].tileType != TileType.Tunnel)
            return;

        if (tileArray[x - 1, y + 1] == null)
            return;

        if (!tileArray[x - 1, y + 1].rightWall)
            return;

        if (tileArray[x, y + 1] == null)
            return;

        if (!tileArray[x, y + 1].leftWall)
            return;

        PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
        Vector3 position = new Vector3(-1f, 0.5f, 1f);
        PlaceObject(poolChild, position, x, y);

        tileArray[x, y].SetTopLeftColumn();
        tileArray[x - 1, y].SetTopRightColumn();
        tileArray[x - 1, y + 1].SetBottomRightColumn();
        tileArray[x, y + 1].SetBottomLeftColumn();

    }

    void TopRightWallSplit(int x, int y)
    {
        if (tileArray[x, y + 1] == null)
            return;

        if (tileArray[x, y + 1].tileType != TileType.Tunnel)
            return;

        if (tileArray[x + 1, y + 1] == null)
            return;

        if (!tileArray[x + 1, y + 1].bottomWall)
            return;

        if (tileArray[x + 1, y] == null)
            return;

        if (!tileArray[x + 1, y].topWall)
            return;

        PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
        Vector3 position = new Vector3(0, 0.5f, 1f);
        PlaceObject(poolChild, position, x, y);

        tileArray[x, y].SetTopRightColumn();
        tileArray[x, y + 1].SetBottomRightColumn();
        tileArray[x + 1, y].SetTopLeftColumn();
        tileArray[x + 1, y + 1].SetBottomLeftColumn();
    }

    void BottomLeftWallSplit(int x, int y)
    {
        if (tileArray[x, y - 1] == null)
            return;

        if (tileArray[x, y - 1].tileType != TileType.Tunnel)
            return;

        if (tileArray[x - 1, y] == null)
            return;

        if (!tileArray[x - 1, y].bottomWall)
            return;

        if (tileArray[x - 1, y - 1] == null)
            return;

        if (!tileArray[x - 1, y - 1].topWall)
            return;

        PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
        Vector3 position = new Vector3(-1, 0.5f, 0);
        PlaceObject(poolChild, position, x, y);

        tileArray[x, y].SetBottomLeftColumn();
        tileArray[x, y - 1].SetTopLeftColumn();
        tileArray[x - 1, y].SetBottomLeftColumn();
        tileArray[x - 1, y - 1].SetTopRightColumn();

    }

    void BottomRightWallSplit(int x, int y)
    {
        if (tileArray[x + 1, y] == null)
            return;

        if (tileArray[x + 1, y].tileType != TileType.Tunnel)
            return;

        if (tileArray[x + 1, y - 1] == null)
            return;

        if (!tileArray[x + 1, y - 1].leftWall)
            return;

        if (tileArray[x, y - 1] == null)
            return;

        if (!tileArray[x, y - 1].rightWall)
            return;

        PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
        Vector3 position = new Vector3(0, 0.5f, 0);
        PlaceObject(poolChild, position, x, y);

        tileArray[x, y].SetBottomRightColumn();
        tileArray[x + 1, y].SetBottomLeftColumn();
        tileArray[x, y - 1].SetTopRightColumn();
        tileArray[x + 1, y - 1].SetTopLeftColumn();
    }
 * 
      if (!array[x, y].biome.StopWallGeneration())
          return;

      if (array[x, y + 1] == null)
          return;


      if (array[x, y + 1].biome.StopWallGeneration())
          return;


      if (!CheckRightWall(x, y + 1, array))
          return;

      if (CheckTopWall(x + 1, y, array))
          return;

      if (array[x +1, y +1] == null)
      {
          //yolo do the wall transition instead
      }
      else
      {
          GameObject poolChild = array[x, y].biome.BigColumnPrefab();
          Vector3 position = new Vector3(0, 0.5f, 1f);
          PlaceObject(poolChild, position, x, y);
      }
}




      /*
      if (!CheckSameBiome(x - 1, y, array, array[x, y].biome))
          return;

      if (!CheckLeftWall(x, y + 1, array))
          return;

      GameObject prefab = array[x, y].biome.BigColumnPrefab();
      Vector3 position = new Vector3(-1f, 0.5f, 1f);
      PlaceObject(prefab, position, x, y);

  }

  void BottomWallSplit(int x, int y, GenerationData[,] array)
  {
      if (!array[x, y].biome.StopWallGeneration())
          return;

      if (array[x, y - 1] == null)
          return;

      /*
      if (array[x, y + 1].biome.StopWallGeneration())
          return;


      if (!CheckRightWall(x, y - 1, array))
          return;

      if (CheckBottomWall(x + 1, y, array))
          return;

      if (array[x + 1, y - 1] == null)
      {
          //yolo do the wall transition instead
      }
      else
      {
          GameObject prefab = array[x, y].biome.BigColumnPrefab();
          Vector3 position = new Vector3(0, 0.5f, 0);
          PlaceObject(prefab, position, x, y);
      }
  }

  void RightWallSplit(int x, int y, GenerationData[,] array)
  {
      if (!array[x, y].biome.StopWallGeneration())
          return;


      if (array[x + 1, y] == null)
          return;

  }
}

      if (!CheckSameBiome(x, y + 1, array, array[x, y].biome))
          return;

      if (!CheckTopWall(x - 1, y, array))
          return;

      GameObject poolChild = array[x, y].biome.BigColumnPrefab();
      Vector3 position = new Vector3(0, 0.5f, 1f);
      PlaceObject(poolChild, position, x, y);
  }
}


*     void TopLeftInnerCorner(int x, int y)
  {
      if (tileArray[x, y].topLeftColumn)
          return;

      if (tileArray[x, y].leftWall)
          return;

      if (tileArray[x, y].topWall)
          return;

      if (tileArray[x - 1, y] == null)
          return;

      if (!tileArray[x - 1, y].topWall)
          return;

      // potential fix 
      //    if (tileArray[x, y].tileType != tileArray[x - 1, y].tileType)
      //        return;

      if (tileArray[x, y + 1] == null)
          return;

      if (!tileArray[x, y + 1].leftWall)
          return;

      // potential fix 
      //    if (tileArray[x, y].tileType != tileArray[x, y + 1].tileType)
      //         return;

      PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
      Vector3 position = new Vector3(-0.925f, 0.5f, 0.925f);
      PlaceObject(poolChild, position, x, y);
      tileArray[x, y].SetTopRightColumn();
  }

  void TopRightInnerCorner(int x, int y)
  {
      if (tileArray[x, y].topRightColumn)
          return;

      if (tileArray[x, y].topWall)
          return;

      if (tileArray[x, y].rightWall)
          return;

      if (tileArray[x, y + 1] == null)
          return;

      if (!tileArray[x, y + 1].rightWall)
          return;

      // potential fix 
      //   if (tileArray[x, y].tileType != tileArray[x, y + 1].tileType)
      //       return;

      if (tileArray[x + 1, y] == null)
          return;

      if (!tileArray[x + 1, y].topWall)
          return;

      // potential fix 
      //   if (tileArray[x, y].tileType != tileArray[x + 1, y].tileType)
      //       return;


      PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
      Vector3 position = new Vector3(-0.075f, 0.5f, 0.925f);
      PlaceObject(poolChild, position, x, y);
      tileArray[x, y].SetTopRightColumn();
  }

  void BottomRightInnerCorner(int x, int y)
  {
      if (tileArray[x, y].bottomRightColumn)
          return;

      if (tileArray[x, y].bottomWall)
          return;

      if (tileArray[x, y].rightWall)
          return;

      if (tileArray[x + 1, y] == null)
          return;

      if (!tileArray[x + 1, y].bottomWall)
          return;

      // potential fix 
      //    if (tileArray[x, y].tileType != tileArray[x + 1, y].tileType)
      //       return;

      if (tileArray[x, y - 1] == null)
          return;

      if (!tileArray[x, y - 1].rightWall)
          return;

      // potential fix 
      //     if (tileArray[x, y].tileType != tileArray[x, y - 1].tileType)
      //        return;

      PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
      Vector3 position = new Vector3(-0.075f, 0.5f, 0.075f);
      PlaceObject(poolChild, position, x, y);
      tileArray[x, y].SetBottomRightColumn();
  }

  void BottomLeftInnerCorner(int x, int y)
  {
      if (tileArray[x, y].bottomLeftColumn)
          return;

      if (tileArray[x, y].bottomWall)
          return;

      if (tileArray[x, y].leftWall)
          return;

      if (tileArray[x - 1, y] == null)
          return;

      if (!tileArray[x - 1, y].bottomWall)
          return;

      // potential fix 
      //     if (tileArray[x, y].tileType != tileArray[x - 1, y].tileType)
      //         return;

      if (tileArray[x, y - 1] == null)
          return;

      if (!tileArray[x, y - 1].leftWall)
          return;

      // potential fix 
      //     if (tileArray[x, y].tileType != tileArray[x, y - 1].tileType)
      //        return;

      PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
      Vector3 position = new Vector3(-0.925f, 0.5f, 0.075f);
      PlaceObject(poolChild, position, x, y);
      tileArray[x, y].SetBottomLeftColumn();
  }

*/






/*
void GenerateTopLeftOutsideCorner(int x, int y, GenerationData[,] array)
{
    if (CheckWall(x, y + 1, array, array[x,y].biome))
        return;

    if (CheckWall(x -1, y, array, array[x, y].biome))
        return;

    GameObject prefab = array[x, y].biome.BigColumnPrefab();
    Vector3 position = new Vector3(-0.85f, 0.5f, 0.85f);
    PlaceObject(prefab, position, x, y);

    //   PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
    //  Vector3 position = new Vector3(-0.85f, 0.5f, 0.85f);
    //   PlaceObject(poolChild, position, x, y);
    //   tileArray[x, y].SetTopLeftColumn();
}
}
*/
/*
void TopRightOutsideCorner(int x, int y)
{
    if (!tileArray[x, y].rightWall)
        return;

    if (!tileArray[x, y].topWall)
        return;

    PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
    Vector3 position = new Vector3(-0.15f, 0.5f, 0.85f);
    PlaceObject(poolChild, position, x, y);
    tileArray[x, y].SetTopRightColumn();
}

void BottomLeftOutsideCorner(int x, int y)
{
    if (!tileArray[x, y].leftWall)
        return;

    if (!tileArray[x, y].bottomWall)
        return;

    PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
    Vector3 position = new Vector3(-0.85f, 0.5f, 0.15f);
    PlaceObject(poolChild, position, x, y);
    tileArray[x, y].SetBottomLeftColumn();
}

void BottomRightOutsideCorner(int x, int y)
{
    if (!tileArray[x, y].rightWall)
        return;

    if (!tileArray[x, y].bottomWall)
        return;

    PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
    Vector3 position = new Vector3(-0.15f, 0.5f, 0.15f);
    PlaceObject(poolChild, position, x, y);
    tileArray[x, y].SetBottomRightColumn();
}
}






*/





/*


void GenerateWallSplits()
    {
        for (int x = 0; x < tileArray.GetLength(0); x++)
        {
            for (int z = 0; z < tileArray.GetLength(1); z++)
            {
                if (tileArray[x, z] == null)
                    continue;

                if (tileArray[x, z].tileType != TileType.Tunnel)
                    continue;

                TopLeftWallSplit(x, z);
                TopRightWallSplit(x, z);
                BottomLeftWallSplit(x, z);
                BottomRightWallSplit(x, z);
            }
        }
    }

  

    void TopLeftWallSplit(int x, int y)
    {
        if (tileArray[x - 1, y] == null)
            return;

        if (tileArray[x - 1, y].tileType != TileType.Tunnel)
            return;

        if (tileArray[x - 1, y + 1] == null)
            return;

        if (!tileArray[x - 1, y + 1].rightWall)
            return;

        if (tileArray[x, y + 1] == null)
            return;

        if (!tileArray[x, y + 1].leftWall)
            return;

        PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
        Vector3 position = new Vector3(-1f, 0.5f, 1f);
        PlaceObject(poolChild, position, x, y);

        tileArray[x, y].SetTopLeftColumn();
        tileArray[x - 1, y].SetTopRightColumn();
        tileArray[x - 1, y + 1].SetBottomRightColumn();
        tileArray[x, y + 1].SetBottomLeftColumn();

    }

    void TopRightWallSplit(int x, int y)
    {
        if (tileArray[x, y + 1] == null)
            return;

        if (tileArray[x, y + 1].tileType != TileType.Tunnel)
            return;

        if (tileArray[x + 1, y + 1] == null)
            return;

        if (!tileArray[x + 1, y + 1].bottomWall)
            return;

        if (tileArray[x + 1, y] == null)
            return;

        if (!tileArray[x + 1, y].topWall)
            return;

        PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
        Vector3 position = new Vector3(0, 0.5f, 1f);
        PlaceObject(poolChild, position, x, y);

        tileArray[x, y].SetTopRightColumn();
        tileArray[x, y + 1].SetBottomRightColumn();
        tileArray[x + 1, y].SetTopLeftColumn();
        tileArray[x + 1, y + 1].SetBottomLeftColumn();
    }

    void BottomLeftWallSplit(int x, int y)
    {
        if (tileArray[x, y - 1] == null)
            return;

        if (tileArray[x, y - 1].tileType != TileType.Tunnel)
            return;

        if (tileArray[x - 1, y] == null)
            return;

        if (!tileArray[x - 1, y].bottomWall)
            return;

        if (tileArray[x - 1, y - 1] == null)
            return;

        if (!tileArray[x - 1, y - 1].topWall)
            return;

        PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
        Vector3 position = new Vector3(-1, 0.5f, 0);
        PlaceObject(poolChild, position, x, y);

        tileArray[x, y].SetBottomLeftColumn();
        tileArray[x, y - 1].SetTopLeftColumn();
        tileArray[x - 1, y].SetBottomLeftColumn();
        tileArray[x - 1, y - 1].SetTopRightColumn();

    }

    void BottomRightWallSplit(int x, int y)
    {
        if (tileArray[x + 1, y] == null)
            return;

        if (tileArray[x + 1, y].tileType != TileType.Tunnel)
            return;

        if (tileArray[x + 1, y - 1] == null)
            return;

        if (!tileArray[x + 1, y - 1].leftWall)
            return;

        if (tileArray[x, y - 1] == null)
            return;

        if (!tileArray[x, y - 1].rightWall)
            return;

        PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
        Vector3 position = new Vector3(0, 0.5f, 0);
        PlaceObject(poolChild, position, x, y);

        tileArray[x, y].SetBottomRightColumn();
        tileArray[x + 1, y].SetBottomLeftColumn();
        tileArray[x, y - 1].SetTopRightColumn();
        tileArray[x + 1, y - 1].SetTopLeftColumn();
    }

    void GenerateTunnelColumns()
    {
        for (int x = 0; x < tileArray.GetLength(0); x++)
        {
            for (int z = 0; z < tileArray.GetLength(1); z++)
            {
                if (tileArray[x, z] == null)
                    continue;

                if (tileArray[x, z].tileType != TileType.Tunnel)
                    continue;

                TopLeftMajorTunnelColumn(x, z);
                TopLeftMinorColumn(x, z);

                TopRightMajorTunnelColumn(x, z);
                TopRightMinorTunnelColumn(x, z); 

                BottomLeftMajorColumn(x, z);
                BottomLeftMinorColumn(x, z);

                BottomRightMajorColumn(x, z);
                BottomRightMinorColumn(x, z);
            }
        }
    }

    void TopLeftMajorTunnelColumn(int x, int y)
    {
        if (tileArray[x, y].topLeftColumn)
            return;

        if (tileArray[x - 1, y] == null)
            return;

        if (tileArray[x - 1, y].tileType == TileType.Tunnel)
            return;

        if (tileArray[x, y + 1] == null)
            return;

        if (tileArray[x, y + 1].tileType == TileType.Tunnel)
            return;

        if (tileArray[x - 1, y + 1] == null)
        {
            Vector3 position = new Vector3(-0.85f, 0.5f, 0.85f);
            PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
            PlaceObject(poolChild, position, x, y);
        }
        else
        {
            Vector3 position = new Vector3(-1f, 0.5f, 1);
            PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
            PlaceObject(poolChild, position, x, y);
            tileArray[x - 1, y + 1].SetBottomRightColumn();
        }

        tileArray[x, y].SetTopLeftColumn();
        tileArray[x - 1, y].SetTopRightColumn();
        tileArray[x, y + 1].SetBottomLeftColumn();
    }

    void TopLeftMinorColumn(int x, int y)
    {
        if (tileArray[x, y].topLeftColumn)
            return;

        if (tileArray[x - 1, y] == null)
            return;

        if (tileArray[x - 1, y].tileType != TileType.Tunnel)
            return;

        if (tileArray[x, y + 1] == null)
            return;

        if (tileArray[x, y + 1].tileType == TileType.Tunnel)
            return;

        Vector3 position = new Vector3(-1f, 0.5f, 1);
        PoolChild poolChild = tileArray[x, y].minorColumnPool.RequestObject();
        PlaceObject(poolChild, position, x, y);

        tileArray[x, y].SetTopLeftColumn();
        tileArray[x - 1, y].SetTopRightColumn();
        tileArray[x, y + 1].SetTopLeftColumn();
    }

    void TopRightMajorTunnelColumn(int x, int y)
    {
        if (tileArray[x, y].topRightColumn)
            return;

        if (tileArray[x, y + 1] == null)
            return;

        if (tileArray[x, y + 1].tileType == TileType.Tunnel)
            return;

        if (tileArray[x + 1, y] == null)
            return;

        if (tileArray[x + 1, y].tileType == TileType.Tunnel)
            return;

        if (tileArray[x + 1, y + 1] == null)
        {
            Vector3 position = new Vector3(-0.15f, 0.5f, 0.85f);
            PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
            PlaceObject(poolChild, position, x, y);
        }
        else
        {
            Vector3 position = new Vector3(0, 0.5f, 1);
            PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
            PlaceObject(poolChild, position, x, y);
            tileArray[x + 1, y + 1].SetBottomLeftColumn();
        }

        tileArray[x, y].SetTopRightColumn();
        tileArray[x + 1, y].SetTopLeftColumn();
        tileArray[x, y + 1].SetBottomRightColumn();
    }

    void TopRightMinorTunnelColumn(int x, int y)
    {
        if (tileArray[x, y].topRightColumn)
            return;

        if (tileArray[x, y + 1] == null)
            return;

        if (tileArray[x, y + 1].tileType != TileType.Tunnel)
            return;

        if (tileArray[x + 1, y] == null)
            return;

        if (tileArray[x + 1, y].tileType == TileType.Tunnel)
            return;

        Vector3 position = new Vector3(0, 0.5f, 1);
        PoolChild poolChild = tileArray[x, y].minorColumnPool.RequestObject();
        PlaceObject(poolChild, position, x, y);

        tileArray[x, y].SetTopRightColumn();
        tileArray[x + 1, y].SetTopLeftColumn();
        tileArray[x, y + 1].SetBottomRightColumn();
    }
    void BottomLeftMajorColumn(int x, int y)
    {
        if (tileArray[x, y].bottomLeftColumn)
            return;

        if (tileArray[x - 1, y] == null)
            return;

        if (tileArray[x - 1, y].tileType == TileType.Tunnel)
            return;

        if (tileArray[x, y - 1] == null)
            return;

        if (tileArray[x, y - 1].tileType == TileType.Tunnel)
            return;

        if (tileArray[x - 1, y - 1] == null)
        {
            Vector3 position = new Vector3(-0.85f, 0.5f, 0.15f);
            PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
            PlaceObject(poolChild, position, x, y);
        }
        else
        {
            Vector3 position = new Vector3(-1f, 0.5f, 0);
            PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
            PlaceObject(poolChild, position, x, y);
            tileArray[x - 1, y - 1].SetTopRightColumn();
        }

        tileArray[x, y].SetBottomLeftColumn();
        tileArray[x - 1, y].SetBottomRightColumn();
        tileArray[x, y - 1].SetTopLeftColumn();
    }

    void BottomLeftMinorColumn(int x, int y)
    {
        if (tileArray[x, y].bottomLeftColumn)
            return;

        if (tileArray[x - 1, y] == null)
            return;

        if (tileArray[x - 1, y].tileType == TileType.Tunnel)
            return;

        if (tileArray[x, y - 1] == null)
            return;

        if (tileArray[x, y - 1].tileType != TileType.Tunnel)
            return;

        Vector3 position = new Vector3(-1f, 0.5f, 0);
        PoolChild poolChild = tileArray[x, y].minorColumnPool.RequestObject();
        PlaceObject(poolChild, position, x, y);

        tileArray[x, y].SetBottomLeftColumn();
        tileArray[x - 1, y].SetBottomRightColumn();
        tileArray[x, y - 1].SetTopLeftColumn();
    }

    void BottomRightMajorColumn(int x, int y)
    {
        if (tileArray[x, y].bottomRightColumn)
            return;

        if (tileArray[x + 1, y] == null)
            return;

        if (tileArray[x + 1, y].tileType == TileType.Tunnel)
            return;

        if (tileArray[x, y - 1] == null)
            return;

        if (tileArray[x, y - 1].tileType == TileType.Tunnel)
            return;

        if (tileArray[x +1, y -1] == null)
        {
            Vector3 position = new Vector3(-0.15f, 0.5f, 0.15f);
            PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
            PlaceObject(poolChild, position, x, y);
        }
        else
        {
            Vector3 position = new Vector3(0f, 0.5f, 0);
            PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
            PlaceObject(poolChild, position, x, y);
            tileArray[x + 1, y - 1].SetTopLeftColumn();
        }

        tileArray[x, y].SetBottomRightColumn();
        tileArray[x + 1, y].SetBottomLeftColumn();
        tileArray[x, y - 1].SetTopRightColumn();
    }


    void BottomRightMinorColumn(int x, int y)
    {
        if (tileArray[x, y].bottomRightColumn)
            return;

        if (tileArray[x + 1, y] == null)
            return;

        if (tileArray[x + 1, y].tileType != TileType.Tunnel)
            return;

        if (tileArray[x, y - 1] == null)
            return;

        if (tileArray[x, y - 1].tileType == TileType.Tunnel)
            return;

        Vector3 position = new Vector3(0f, 0.5f, 0);
        PoolChild poolChild = tileArray[x, y].minorColumnPool.RequestObject();
        PlaceObject(poolChild, position, x, y);

        tileArray[x, y].SetBottomRightColumn();
        tileArray[x + 1, y].SetBottomLeftColumn();
        tileArray[x, y - 1].SetTopRightColumn();

    }

    void GenerateTileSplits()
    {
        for (int x = 0; x < tileArray.GetLength(0); x++)
        {
            for (int z = 0; z < tileArray.GetLength(1); z++)
            {
                if (tileArray[x, z] == null)
                    continue;

                if (tileArray[x, z].tileType != TileType.Tunnel)
                    continue;

                    NorthLeftSplit(x, z);
                    NorthRightSplit(x, z);

                    SouthLeftSplit(x, z);
                   SouthRightSplit(x, z);

                      EastTopSplit(x, z);
                     EastBottomSplit(x, z);

                WestTopSplit(x, z);
                WestBottomSplit(x, z);

            }
        }
    }

    void NorthLeftSplit(int x, int y)
    {
        if (tileArray[x, y].topLeftColumn)
            return;

        if (tileArray[x, y + 1] == null)
            return;

        if (tileArray[x, y + 1].tileType == TileType.Tunnel)
            return;

        if (!tileArray[x, y + 1].leftWall)
            return;

        Vector3 position;
        if (tileArray[x, y].leftWall)
        {            
            position = new Vector3(-0.85f, 0.5f, 1);
            
        }
        else
        {
            position = new Vector3(-1f, 0.5f, 1);
        }

        PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
        PlaceObject(poolChild, position, x, y);
        tileArray[x, y].SetTopLeftColumn();
        tileArray[x, y + 1].SetBottomLeftColumn();
    }

    void NorthRightSplit(int x, int y)
    {
        if (tileArray[x, y].topRightColumn)
            return;

        if (tileArray[x, y + 1] == null)
            return;

        if (tileArray[x, y + 1].tileType == TileType.Tunnel)
            return;

        if (!tileArray[x, y + 1].rightWall)
            return;

        Vector3 position;
        if (tileArray[x,y].rightWall)
        {
            position = new Vector3(-0.15f, 0.5f, 1);
        }
        else
        {
            position = new Vector3(0, 0.5f, 1);
        }

        PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();        
        PlaceObject(poolChild, position, x, y);
        tileArray[x, y].SetTopRightColumn();
        tileArray[x, y + 1].SetBottomRightColumn();
    }

    void SouthLeftSplit(int x, int y)
    {
        if (tileArray[x, y].bottomLeftColumn)
            return;

        if (tileArray[x, y - 1] == null)
            return;

        if (tileArray[x, y - 1].tileType == TileType.Tunnel)
            return;

        if (!tileArray[x, y - 1].leftWall)
            return;

        Vector3 position;
        if (tileArray[x,y].leftWall)
        {
            position = new Vector3(-0.85f, 0.5f, 0);
        }
        else
        {
            position = new Vector3(-1f, 0.5f, 0);
        }

        PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();        
        PlaceObject(poolChild, position, x, y);
        tileArray[x, y].SetBottomLeftColumn();
        tileArray[x, y - 1].SetTopLeftColumn();
    }

    void SouthRightSplit(int x, int y)
    {
        if (tileArray[x, y].bottomRightColumn)
            return;

        if (tileArray[x, y - 1] == null)
            return;

        if (tileArray[x, y - 1].tileType == TileType.Tunnel)
            return;

        if (!tileArray[x, y - 1].rightWall)
            return;

        Vector3 position;
        if (tileArray[x, y].rightWall)
        {
            position = new Vector3(-0.15f, 0.5f, 0);
        }
        else
        {
            position = new Vector3(0, 0.5f, 0);
        }

        PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();        
        PlaceObject(poolChild, position, x, y);
        tileArray[x, y].SetBottomRightColumn();
        tileArray[x, y - 1].SetTopRightColumn();
    }

    void EastTopSplit(int x, int y)
    {
        if (tileArray[x, y].topRightColumn)
            return;

        if (tileArray[x + 1, y] == null)
            return;

        if (tileArray[x + 1, y].tileType == TileType.Tunnel)
            return;

        if (!tileArray[x + 1, y].topWall)
            return;

        Vector3 position;
        if(tileArray[x, y].topWall)
        {
            position = new Vector3(0, 0.5f, 0.85f);
        }
        else
        {
            position = new Vector3(0, 0.5f, 1);
        }

        PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();        
        PlaceObject(poolChild, position, x, y);
        tileArray[x, y].SetTopRightColumn();
        tileArray[x + 1, y].SetTopRightColumn();
    }

    void EastBottomSplit(int x, int y)
    {
        if (tileArray[x, y].bottomRightColumn)
            return;

        if (tileArray[x + 1, y] == null)
            return;

        if (tileArray[x + 1, y].tileType == TileType.Tunnel)
            return;

        if (!tileArray[x + 1, y].bottomWall)
            return;

        Vector3 position;
        if (tileArray[x, y].bottomWall)
        {
            position = new Vector3(0, 0.5f, 0.15f);
        }
        else
        {
            position = new Vector3(0, 0.5f, 0);
        }

        PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
        PlaceObject(poolChild, position, x, y);
        tileArray[x, y].SetBottomRightColumn();
        tileArray[x + 1, y].SetBottomLeftColumn();
    }

    void WestTopSplit(int x, int y)
    {
        if (tileArray[x, y].topLeftColumn)
            return;

        if (tileArray[x - 1, y] == null)
            return;

        if (tileArray[x - 1, y].tileType == TileType.Tunnel)
            return;

        if (!tileArray[x - 1, y].topWall)
            return;

        Vector3 position;
        if(tileArray[x, y].topWall)
        {
            position = new Vector3(-1, 0.5f, 0.85f);
        }
        else
        {
            position = new Vector3(-1, 0.5f, 1);
        }

        PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();        
        PlaceObject(poolChild, position, x, y);

        tileArray[x, y].SetTopLeftColumn();
        tileArray[x - 1, y].SetTopRightColumn();
    }

    void WestBottomSplit(int x, int y)
    {
        if (tileArray[x, y].bottomLeftColumn)
            return;

        if (tileArray[x - 1, y] == null)
            return;

        if (tileArray[x - 1, y].tileType == TileType.Tunnel)
            return;

        if (!tileArray[x - 1, y].bottomWall)
            return;

        Vector3 position;
        if (tileArray[x, y].bottomWall)
        {
            position = new Vector3(-1, 0.5f, 0.15f);
        }
        else
        {
            position = new Vector3(-1, 0.5f, 0);
        }

        PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
        PlaceObject(poolChild, position, x, y);

        tileArray[x, y].SetBottomLeftColumn();
        tileArray[x - 1, y].SetBottomRightColumn();
    }


    void GenerateOutsideCorners()
    {
        for (int x = 0; x < tileArray.GetLength(0); x++)
        {
            for (int z = 0; z < tileArray.GetLength(1); z++)
            {
                if (tileArray[x, z] == null)
                    continue;


                TopLeftOutsideCorner(x, z);
                BottomRightOutsideCorner(x, z);
                TopRightOutsideCorner(x, z);
                BottomLeftOutsideCorner(x, z);
            }
        }
    }

    void TopLeftOutsideCorner(int x, int y)
    {
        if (!tileArray[x, y].leftWall)
            return;

        if (!tileArray[x, y].topWall)
            return;

        PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
        Vector3 position = new Vector3(-0.85f, 0.5f, 0.85f);
        PlaceObject(poolChild, position, x, y);
        tileArray[x, y].SetTopLeftColumn();
    }

    void TopRightOutsideCorner(int x, int y)
    {
        if (!tileArray[x, y].rightWall)
            return;

        if (!tileArray[x, y].topWall)
            return;

        PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
        Vector3 position = new Vector3(-0.15f, 0.5f, 0.85f);
        PlaceObject(poolChild, position, x, y);
        tileArray[x, y].SetTopRightColumn();
    }

    void BottomLeftOutsideCorner(int x, int y)
    {
        if (!tileArray[x, y].leftWall)
            return;

        if (!tileArray[x, y].bottomWall)
            return;

        PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
        Vector3 position = new Vector3(-0.85f, 0.5f, 0.15f);
        PlaceObject(poolChild, position, x, y);
        tileArray[x, y].SetBottomLeftColumn();
    }

    void BottomRightOutsideCorner(int x, int y)
    {
        if (!tileArray[x, y].rightWall)
            return;

        if (!tileArray[x, y].bottomWall)
            return;

        PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
        Vector3 position = new Vector3(-0.15f, 0.5f, 0.15f);
        PlaceObject(poolChild, position, x, y);
        tileArray[x, y].SetBottomRightColumn();
    }

    void GenerateInnerCorners()
    {
        for (int x = 0; x < tileArray.GetLength(0); x++)
        {
            for (int z = 0; z < tileArray.GetLength(1); z++)
            {
                if (tileArray[x, z] == null)
                    continue;

                TopLeftInnerCorner(x, z);
                TopRightInnerCorner(x, z);
                BottomLeftInnerCorner(x, z);
                BottomRightInnerCorner(x, z);
            }
        }
    }

    void TopLeftInnerCorner(int x, int y)
    {
        if (tileArray[x, y].topLeftColumn)
            return;

        if (tileArray[x, y].leftWall)
            return;

        if (tileArray[x, y].topWall)
            return;

        if (tileArray[x - 1, y] == null)
            return;

        if (!tileArray[x - 1, y].topWall)
            return;

        // potential fix 
        //    if (tileArray[x, y].tileType != tileArray[x - 1, y].tileType)
        //        return;

        if (tileArray[x, y + 1] == null)
            return;

        if (!tileArray[x, y + 1].leftWall)
            return;

        // potential fix 
        //    if (tileArray[x, y].tileType != tileArray[x, y + 1].tileType)
        //         return;

        PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
        Vector3 position = new Vector3(-0.925f, 0.5f, 0.925f);
        PlaceObject(poolChild, position, x, y);
        tileArray[x, y].SetTopRightColumn();
    }

    void TopRightInnerCorner(int x, int y)
    {
        if (tileArray[x, y].topRightColumn)
            return;

        if (tileArray[x, y].topWall)
            return;

        if (tileArray[x, y].rightWall)
            return;

        if (tileArray[x, y + 1] == null)
            return;

        if (!tileArray[x, y + 1].rightWall)
            return;

        // potential fix 
        //   if (tileArray[x, y].tileType != tileArray[x, y + 1].tileType)
        //       return;

        if (tileArray[x + 1, y] == null)
            return;

        if (!tileArray[x + 1, y].topWall)
            return;

        // potential fix 
        //   if (tileArray[x, y].tileType != tileArray[x + 1, y].tileType)
        //       return;


        PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
        Vector3 position = new Vector3(-0.075f, 0.5f, 0.925f);
        PlaceObject(poolChild, position, x, y);
        tileArray[x, y].SetTopRightColumn();
    }

    void BottomRightInnerCorner(int x, int y)
    {
        if (tileArray[x, y].bottomRightColumn)
            return;

        if (tileArray[x, y].bottomWall)
            return;

        if (tileArray[x, y].rightWall)
            return;

        if (tileArray[x + 1, y] == null)
            return;

        if (!tileArray[x + 1, y].bottomWall)
            return;

        // potential fix 
        //    if (tileArray[x, y].tileType != tileArray[x + 1, y].tileType)
        //       return;

        if (tileArray[x, y - 1] == null)
            return;

        if (!tileArray[x, y - 1].rightWall)
            return;

        // potential fix 
        //     if (tileArray[x, y].tileType != tileArray[x, y - 1].tileType)
        //        return;

        PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
        Vector3 position = new Vector3(-0.075f, 0.5f, 0.075f);
        PlaceObject(poolChild, position, x, y);
        tileArray[x, y].SetBottomRightColumn();
    }

    void BottomLeftInnerCorner(int x, int y)
    {
        if (tileArray[x, y].bottomLeftColumn)
            return;

        if (tileArray[x, y].bottomWall)
            return;

        if (tileArray[x, y].leftWall)
            return;

        if (tileArray[x - 1, y] == null)
            return;

        if (!tileArray[x - 1, y].bottomWall)
            return;

        // potential fix 
        //     if (tileArray[x, y].tileType != tileArray[x - 1, y].tileType)
        //         return;

        if (tileArray[x, y - 1] == null)
            return;

        if (!tileArray[x, y - 1].leftWall)
            return;

        // potential fix 
        //     if (tileArray[x, y].tileType != tileArray[x, y - 1].tileType)
        //        return;

        PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
        Vector3 position = new Vector3(-0.925f, 0.5f, 0.075f);
        PlaceObject(poolChild, position, x, y);
        tileArray[x, y].SetBottomLeftColumn();
    }

    void GenerateColumns()
    {
        for (int x = 0; x < tileArray.GetLength(0); x++)
        {
            for (int z = 0; z < tileArray.GetLength(1); z++)
            {
                if (tileArray[x, z] == null)
                    continue;

                TopLeftColumn(x, z);
                TopRightColumn(x, z);
                BottomLeftColumn(x, z);
                BottomRightColumn(x, z);
            }
        }
    }

    

    void TopLeftColumn(int x, int y)
    {
        if (tileArray[x, y].topLeftColumn)
            return;

        if (tileArray[x, y].leftWall)
            return;

        if (tileArray[x - 1, y] == null)
            return;

        if (!tileArray[x - 1, y].topWall)
            return;

        PoolChild poolChild = tileArray[x, y].minorColumnPool.RequestObject();
        Vector3 position = new Vector3(-1, 0.5f, 0.85f);
        PlaceObject(poolChild, position, x, y);
        tileArray[x, y].SetTopRightColumn();
    }


    void TopRightColumn(int x, int y)
    {
        if (tileArray[x, y].topRightColumn)
            return;

        if (tileArray[x, y].topWall)
            return;

        if (tileArray[x, y + 1] == null)
            return;

        if (tileArray[x, y + 1].rightWall)
        {
            PoolChild poolChild = tileArray[x, y].minorColumnPool.RequestObject();
            Vector3 position = new Vector3(-0.15f, 0.5f, 1);
            PlaceObject(poolChild, position, x, y);
            tileArray[x, y].SetTopRightColumn();
        }
    }

    void BottomLeftColumn(int x, int y)
    {
        if (tileArray[x, y].bottomLeftColumn)
            return;

        if (tileArray[x, y].bottomWall)
            return;

        if (tileArray[x, y - 1] == null)
            return;

        if (tileArray[x, y - 1].leftWall)
        {
            PoolChild poolChild = tileArray[x, y].minorColumnPool.RequestObject();
            Vector3 position = new Vector3(-0.85f, 0.5f, 0);
            PlaceObject(poolChild, position, x, y);
            tileArray[x, y].SetBottomLeftColumn();
        }
    }

    void BottomRightColumn(int x, int y)
    {
        if (tileArray[x, y].bottomRightColumn)
            return;

        if (tileArray[x, y].rightWall)
            return;

        if (tileArray[x + 1, y] == null)
            return;

        if (tileArray[x + 1, y].bottomWall)
        {
            PoolChild poolChild = tileArray[x, y].minorColumnPool.RequestObject();
            Vector3 position = new Vector3(0, 0.5f, 0.15f);
            PlaceObject(poolChild, position, x, y);
            tileArray[x, y].SetBottomRightColumn();
        }
    }
}

    */