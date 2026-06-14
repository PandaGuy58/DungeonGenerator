using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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

    void ReturnToPool(List<PoolChild> poolChildList)
    {
        for (int i = 0; i < poolChildList.Count; i++)
        {
            poolChildList[i].ReturnChildToPool();
        }
        poolChildList.Clear();
    }

    void PlaceObject(PoolChild poolChild, Vector3 position, int x, int y)
    {
        Vector3 calculate = tileArray[x, y].transform.position;
        calculate += position;
        poolChild.gameObject.transform.position = calculate;
        contents.Add(poolChild);
    }

    void PlaceObjectRotate(PoolChild poolChild, Vector3 position, Vector3 rotation, int x, int y)
    {
        Vector3 calculate = tileArray[x, y].transform.position;
        calculate += position;
        poolChild.gameObject.transform.position = calculate;
        poolChild.gameObject.transform.eulerAngles = rotation;
        contents.Add(poolChild);
    }

    public void RegenerateTiles()
    {
        GenerationData[,] array = ObjectArray.instance.RequestTemporaryArray();
        ReturnToPool(generatedTiles);
        tileArray = new TileMasterClass[51, 51];
        for (int x = 0; x < array.GetLength(0); x++)
        {
            for (int z = 0; z < array.GetLength(1); z++)
            {
                if (array[x, z] == null)
                    continue;

                PoolChild newTile = array[x, z].pool.RequestObject();
                newTile.transform.position = new Vector3(x + 0.5f, 0.1f, z - 0.5f);
                generatedTiles.Add(newTile);

                if (newTile.IsDestructive())
                    continue;

                TileMasterClass tile = newTile.GetComponent<TileMasterClass>();
                tile.ResetWallsColumns();
                tile.SetXY(x, z);

                tileArray[x, z] = tile;

                if (array[x, z].destruction)
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

    public void DisableContents()
    {
        ReturnToPool(contents);
    }

    public void GenerateContents()
    {
        GenerateWalls();

        GenerateWallSplits();
        GenerateTileSplits();

      //  GenerateInnerCorners();
        //
        //
     //   

         //     GenerateOutsideCorners();
        //    GenerateInnerCorners();
        //     GenerateColumns();
    }


    

    void GenerateWalls()
    {
        for (int x = 0; x < tileArray.GetLength(0); x++)
        {
            for (int z = 0; z < tileArray.GetLength(1); z++)
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
        if (tileArray[x, y + 1] == null)
        {
            TopWall(x, y);
            return;
        }

        if (tileArray[x, y].tileType == tileArray[x, y + 1].tileType)
            return;

        if (tileArray[x, y].tileType == TileType.Tunnel)
            return;

        if (tileArray[x, y + 1].tileType == TileType.Tunnel)
            return;

        TopWall(x, y);
    }

    void TopWall(int x, int y)
    {
        PoolChild poolChild = tileArray[x, y].wallPool.RequestObject();
        Vector3 calculate = Vector3.zero;
        PlaceObjectRotate(poolChild, calculate, calculate, x, y);
        tileArray[x, y].SetTopWall();
    }

    void GenerateBottomWall(int x, int y)
    {
        if (tileArray[x, y - 1] == null)
        {
            BottomWall(x, y);
            return;
        }

        if (tileArray[x, y].tileType == tileArray[x, y - 1].tileType)
            return;

        if (tileArray[x, y].tileType == TileType.Tunnel)
            return;

        if (tileArray[x, y - 1].tileType == TileType.Tunnel)
            return;

        BottomWall(x, y);
    }

    void BottomWall(int x, int y)
    {
        PoolChild poolChild = tileArray[x, y].wallPool.RequestObject();
        Vector3 position = new Vector3(-1, 0, +1);
        Vector3 rotation = new Vector3(0, 180, 0);
        PlaceObjectRotate(poolChild, position, rotation, x, y);
        tileArray[x, y].SetBottomWall();
    }

    void GenerateRightWall(int x, int y)
    {
        if (tileArray[x + 1, y] == null)
        {
            RightWall(x, y);
            return;
        }

        if (tileArray[x, y].tileType == tileArray[x + 1, y].tileType)
            return;

        if (tileArray[x, y].tileType == TileType.Tunnel)
            return;

        if (tileArray[x + 1, y].tileType == TileType.Tunnel)
            return;

        RightWall(x, y);
    }

    void RightWall(int x, int y)
    {
        PoolChild poolChild = tileArray[x, y].wallPool.RequestObject();
        Vector3 position = new Vector3(-1, 0, 0);
        Vector3 rotation = new Vector3(0, 90, 0);
        PlaceObjectRotate(poolChild, position, rotation, x, y);
        tileArray[x, y].SetRightWall();
    }

    void GenerateLeftWall(int x, int y)
    {
        if (tileArray[x - 1, y] == null)
        {
            LeftWall(x, y);
            return;
        }

        if (tileArray[x, y].tileType == tileArray[x - 1, y].tileType)
            return;

        if (tileArray[x, y].tileType == TileType.Tunnel)
            return;

        if (tileArray[x - 1, y].tileType == TileType.Tunnel)
            return;

        LeftWall(x, y);
    }

    void LeftWall(int x, int y)
    {
        PoolChild poolChild = tileArray[x, y].wallPool.RequestObject();
        Vector3 position = new Vector3(0, 0, 1);
        Vector3 rotation = new Vector3(0, -90, 0);
        PlaceObjectRotate(poolChild, position, rotation, x, y);
        tileArray[x, y].SetLeftWall();
    }

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

        tileArray[x, y].topLeftColumn = true;
        tileArray[x - 1, y].topRightColumn = true;
        tileArray[x - 1, y + 1].bottomRightColumn = true;
        tileArray[x, y + 1].bottomLeftColumn = true;

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

        tileArray[x, y].topRightColumn = true;
        tileArray[x, y + 1].bottomRightColumn = true;
        tileArray[x + 1, y].topLeftColumn = true;
        tileArray[x + 1, y + 1].bottomLeftColumn = true;
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

        tileArray[x, y].bottomLeftColumn = true;
        tileArray[x, y - 1].topLeftColumn = true;
        tileArray[x - 1, y].bottomRightColumn = true;
        tileArray[x - 1, y - 1].topRightColumn = true;

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

        tileArray[x, y].bottomRightColumn = true;
        tileArray[x + 1, y].bottomLeftColumn = true;
        tileArray[x, y - 1].topRightColumn = true;
        tileArray[x + 1, y - 1].topLeftColumn = true;
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
        tileArray[x, y].topLeftColumn = true;
        tileArray[x, y + 1].bottomLeftColumn = true;
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
        tileArray[x, y].topRightColumn = true;
        tileArray[x, y + 1].bottomRightColumn = true;
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
        tileArray[x, y].bottomLeftColumn = true;
        tileArray[x, y - 1].topLeftColumn = true;
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
        tileArray[x, y].bottomRightColumn = true;
        tileArray[x, y - 1].topRightColumn = true;
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
        tileArray[x, y].topRightColumn = true;
        tileArray[x + 1, y].topLeftColumn = true;
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
        tileArray[x, y].bottomRightColumn = true;
        tileArray[x + 1, y].bottomLeftColumn = true;
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

        tileArray[x, y].topLeftColumn = true;
        tileArray[x - 1, y].topRightColumn = true;
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

        tileArray[x, y].bottomLeftColumn = true;
        tileArray[x - 1, y].bottomRightColumn = true;
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
        tileArray[x, y].topLeftColumn = true;
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
        tileArray[x, y].topRightColumn = true;
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
        tileArray[x, y].bottomLeftColumn = true;
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
        tileArray[x, y].bottomRightColumn = true;
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

    //    if (tileArray[x, y].tileType != tileArray[x - 1, y].tileType)
    //        return;

        if (tileArray[x, y + 1] == null)
            return;

        if (!tileArray[x, y + 1].leftWall)
            return;

    //    if (tileArray[x, y].tileType != tileArray[x, y + 1].tileType)
   //         return;

        PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
        Vector3 position = new Vector3(-0.925f, 0.5f, 0.925f);
        PlaceObject(poolChild, position, x, y);
        tileArray[x, y].topLeftColumn = true;
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

     //   if (tileArray[x, y].tileType != tileArray[x, y + 1].tileType)
     //       return;

        if (tileArray[x + 1, y] == null)
            return;

        if (!tileArray[x + 1, y].topWall)
            return;

     //   if (tileArray[x, y].tileType != tileArray[x + 1, y].tileType)
     //       return;


        PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
        Vector3 position = new Vector3(-0.075f, 0.5f, 0.925f);
        PlaceObject(poolChild, position, x, y);
        tileArray[x, y].topRightColumn = true;
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

    //    if (tileArray[x, y].tileType != tileArray[x + 1, y].tileType)
     //       return;

        if (tileArray[x, y - 1] == null)
            return;

        if (!tileArray[x, y - 1].rightWall)
            return;

   //     if (tileArray[x, y].tileType != tileArray[x, y - 1].tileType)
    //        return;

        PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
        Vector3 position = new Vector3(-0.075f, 0.5f, 0.075f);
        PlaceObject(poolChild, position, x, y);
        tileArray[x, y].bottomRightColumn = true;
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

   //     if (tileArray[x, y].tileType != tileArray[x - 1, y].tileType)
   //         return;

        if (tileArray[x, y - 1] == null)
            return;

        if (!tileArray[x, y - 1].leftWall)
            return;

   //     if (tileArray[x, y].tileType != tileArray[x, y - 1].tileType)
    //        return;

        PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
        Vector3 position = new Vector3(-0.925f, 0.5f, 0.075f);
        PlaceObject(poolChild, position, x, y);
        tileArray[x, y].bottomLeftColumn = true;
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
        tileArray[x, y].topLeftColumn = true;

        poolChild.columnName = "top left column";
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
            tileArray[x, y].topRightColumn = true;

            poolChild.columnName = "top right column";
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
            tileArray[x, y].bottomLeftColumn = true;

            poolChild.columnName = "bottom left column";
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
            tileArray[x, y].bottomRightColumn = true;

            poolChild.columnName = "bottom right column";
        }
    }
}











/*
void TopLeftTileSplit(int x, int y)
{
    if (tileArray[x, y].topLeftColumn)
        return;

    if (tileArray[x, y + 1] == null)
        return;

    if (tileArray[x, y + 1].tileType == TileType.Tunnel)
        return;

    if (!tileArray[x, y + 1].leftWall)
        return;

    PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
    Vector3 position = new Vector3(-0.85f, 0.5f, 1f);
    PlaceObject(poolChild, position, x, y);

    tileArray[x, y].topLeftColumn = true;
    tileArray[x, y + 1].bottomLeftColumn = true;
}

void TopRightTileSplit(int x, int y)
{
    Debug.Log(Time.time);
    if (tileArray[x, y].topRightColumn)
        return;

    if (tileArray[x + 1, y] == null)
        return;

    if (tileArray[x + 1, y].tileType == TileType.Tunnel)
        return;

    if (!tileArray[x + 1, y].topWall)
        return;

    PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
    Vector3 position = new Vector3(0.0f, 0.5f, 0.85f);
    PlaceObject(poolChild, position, x, y);

    tileArray[x, y].topRightColumn = true;
    tileArray[x + 1, y].topLeftColumn = true;
}


void BottomRightTileSplit(int x, int y)
{
    Debug.Log (Time.time);
    if (tileArray[x, y].bottomRightColumn)
        return;

    if (tileArray[x, y - 1] == null)
        return;

    if (tileArray[x, y - 1].tileType == TileType.Tunnel)
        return;

    if (!tileArray[x, y - 1].rightWall)
        return;

    tileArray[x, y].bottomRightColumn = true;
    tileArray[x, y - 1].topRightColumn = true;

    PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
    Vector3 position = new Vector3(-0.15f, 0.5f, 0);
    PlaceObject(poolChild, position, x, y);
}



void BottomLeftTileSplit(int x, int y)
{
    Debug.Log(Time.time);
    if (tileArray[x, y].bottomLeftColumn)
        return;

    if (tileArray[x - 1, y] == null)
        return;

    if (tileArray[x - 1, y].tileType == TileType.Tunnel)
        return;

    if (!tileArray[x - 1, y].bottomWall)
        return;

    PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
    Vector3 position = new Vector3(-1, 0.5f, 0.15f);
    PlaceObject(poolChild, position, x, y);

    tileArray[x, y].bottomLeftColumn = true;
    tileArray[x - 1, y].bottomRightColumn = true;
}

*/













/*

void GenerateTopLeftMinorColumn(int x, int y)
{
    if (tileArray[x - 1, y] == null)
        return;

    if (tileArray[x - 1, y].topWall)
    {
        PoolChild poolChild = tileArray[x, y].minorColumnPool.RequestObject();
        Vector3 position = new Vector3(-1, 0.5f, 0.85f);
        PlaceObject(poolChild, position, x, y);
        tileArray[x, y].topLeftColumn = true;
    }
}

void GenerateTopRightMinorColumn(int x, int y)
{
    if (tileArray[x, y + 1] == null)
        return;

    if (tileArray[x, y + 1].rightWall)
    {
        PoolChild poolChild = tileArray[x, y].minorColumnPool.RequestObject();
        Vector3 position = new Vector3(-0.15f, 0.5f, 0.85f);
        PlaceObject(poolChild, position, x, y);
        tileArray[x, y].topRightColumn = true;
    }
}

void GenerateBottomRightMinorColumn(int x, int y)
{
    if (tileArray[x + 1, y] == null)
        return;

    if (tileArray[x + 1, y].bottomWall)
    {
        PoolChild poolChild = tileArray[x, y].minorColumnPool.RequestObject();
        Vector3 position = new Vector3(0, 0.5f, 0.15f);
        PlaceObject(poolChild, position, x, y);
        tileArray[x, y].bottomRightColumn = true;
    }
}

void GenerateBottomLeftMinorColumn(int x, int y)
{
    if (tileArray[x, y - 1] == null)
        return;

    if (tileArray[x, y - 1].leftWall)
    {
        PoolChild poolChild = tileArray[x, y].minorColumnPool.RequestObject();
        Vector3 position = new Vector3(-0.85f, 0.5f, 0);
        PlaceObject(poolChild, position, x, y);
        tileArray[x, y].bottomLeftColumn = true;
    }
}


void GenerateTopLeftMajorColumn(int x, int y)
{
    if (tileArray[x, y].topLeftColumn)
        return;

    if (tileArray[x, y].topWall && tileArray[x, y].leftWall)
    {
        PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
        Vector3 position = new Vector3(-0.85f, 0.5f, 0.85f);
        PlaceObject(poolChild, position, x, y);
        return;
    }

    if (tileArray[x - 1, y] != null && tileArray[x, y + 1] != null)
    {
        if (tileArray[x - 1, y].topWall && tileArray[x, y + 1].leftWall)
        {
            PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
            Vector3 position = new Vector3(-0.925f, 0.5f, 0.925f);
            PlaceObject(poolChild, position, x, y);
            return;
        }
    }
}

void GenerateTopRightMajorColumn(int x, int y)
{
    if (tileArray[x, y].topRightColumn)
        return;


    if (tileArray[x, y].topWall && tileArray[x, y].rightWall)
    {
        PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
        Vector3 position = new Vector3(-0.075f, 0.5f, 0.925f);
        PlaceObject(poolChild, position, x, y);
        return;
    }


}

    /*
    if (tileArray[x, y + 1] != null)
    {
        if (tileArray[x, y + 1].rightWall)
        {
            PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
            Vector3 position = new Vector3(-0.15f, 0.5f, 1);
            PlaceObject(poolChild, position, x, y);
            return;
        }
    }
}




void GenerateBottomLeftMajorColumn(int x, int y)
{
    if (tileArray[x, y].bottomLeftColumn)
        return;

    if (tileArray[x, y].bottomWall && tileArray[x, y].leftWall)
    {
        PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
        Vector3 position = new Vector3(-0.85f, 0.5f, 0.15f);
        PlaceObject(poolChild, position, x, y);
        return;
    }

    if (tileArray[x - 1, y] != null && tileArray[x, y - 1] != null)
    {
        if (tileArray[x - 1, y].bottomWall && tileArray[x, y - 1].leftWall)
        {
            PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
            Vector3 position = new Vector3(-0.925f, 0.5f, 0.075f);
            PlaceObject(poolChild, position, x, y);
            return;
        }
    }
}

void GenerateBottomRightMajorColumn(int x, int y)
{
    if (tileArray[x, y].bottomRightColumn)
        return;

    if (tileArray[x, y].bottomWall && tileArray[x, y].rightWall)
    {
        PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
        Vector3 position = new Vector3(-0.15f, 0.5f, 0.15f);
        PlaceObject(poolChild, position, x, y);
        return;
    }

    if (tileArray[x + 1, y] != null && tileArray[x, y - 1] != null)
    {
        if (tileArray[x + 1, y].bottomWall && tileArray[x, y - 1].rightWall)
        {
            PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
            Vector3 position = new Vector3(-0.075f, 0.5f, 0.075f);
            PlaceObject(poolChild, position, x, y);
            return;
        }
    }
}
}




*/







/*
if (tileArray[x + 1, y] != null)
{
    if (tileArray[x + 1, y].bottomWall)
    {
        PoolChild poolChild = tileArray[x, y].minorColumnPool.RequestObject();
        Vector3 position = new Vector3(0, 0.5f, 0.15f);
        PlaceObject(poolChild, position, x, y);
        return;
    }
}
}
}

*/



/*




    //   Debug.Log(tileArray[x - 1, y].topWall + " " + Time.time);
    //      Debug.Log(tileArray[x, y + 1].leftWall + " " + Time.time);  
    //     if (tileArray[x - 1, y].topWall && tileArray[x, y + 1].leftWall)
    //   {
    //     Debug.Log(Time.time);
    //     PoolChild poolChild = tileArray[x, y].majorColumnPool.RequestObject();
    //////     Vector3 position = new Vector3(-0.85f, 0.5f, 0.85f);
    //    PlaceObject(poolChild, position, x, y);
    //    return;
    // }

    /*
     *      calculate = tileArray[x, y].transform.position;
     calculate.x -= 0.85f;
     calculate.y += 0.5f;
     calculate.z += 0.85f;

     poolChild = tileArray[x, y].majorColumnPool.RequestObject();
     poolChild.transform.position = calculate;
     contents.Add(poolChild);
     return;
    */

/*
if (tileArray[x, y].tileType == TileType.Tunnel)
{

}
else
{

}
}

void TopLeftTunnelColumn(int x, int y)
{

}

void TopLeftRegularColumn(int x, int y)
{

}


bool CheckNullOrDifferentTile(TileMasterClass tile, int x, int y)
{
    if (tileArray[x, y] == null)
        return true;

    if (tileArray[x, y].tileType == TileType.Tunnel)
        return false;

    if (tileArray[x, y].tileType != tile.tileType)
        return true;

    return false;
}





void GenerateTopLeftCorner(int x, int y)
{
    if (tileArray[x, y + 1] == null && tileArray[x - 1, y] == null)
    {
        TopLeftCorner(x, y);
        return;
    }

    if (tileArray[x, y].tileType == TileType.Tunnel)
    {
        TunnelTopLeftCorner(x, y);
    }
    else
    {
        GenericTopLeftCorner(x, y); 
    }
}








if(tileArray[x, y + 1] == null && tileArray[x - 1, y] == null)
{
  TopLeftCorner(x, y);
  return;
}

if (CheckTunnel(x, y + 1) && CheckTunnel(x - 1, y))
{
  TopLeftCorner(x, y);
  return;
}

//   if(!CheckTile(tileArray[x, y], x, y + 1) && !CheckTile(tileArray[x, y], x - 1, y))
{
 calculate = tileArray[x, y].transform.position;
 calculate.x -= 0.85f;
 calculate.y += 0.5f;
 calculate.z += 0.85f;

 poolChild = tileArray[x, y].majorColumnPool.RequestObject();
 poolChild.transform.position = calculate;
 contents.Add(poolChild);
 return;
}

if (!CheckTile(tileArray[x, y], x, y + 1))
 return;

if (!CheckTile(tileArray[x, y], x - 1, y))
 return;

if (CheckTile(tileArray[x, y], x - 1, y + 1))
 return;


calculate = tileArray[x, y].transform.position;
calculate.x -= 0.85f;
calculate.y += 0.5f;
calculate.z += 0.85f;

poolChild = tileArray[x, y].majorColumnPool.RequestObject();
poolChild.transform.position = calculate;
contents.Add(poolChild);





bool CheckSameTile(TileMasterClass tile, int x, int y)
{
    if (tileArray[x, y] == null)
        return false;

    if (tile.tileType == tileArray[x, y].tileType)
        return true;

    return false;
}

TileType CheckTileType(int x, int y)
{
    if (tileArray[x, y] == null)
        return TileType.Null;

    return tileArray[x, y].tileType;
}

bool TunnelOrNull(TileType type)
{
    if (type == TileType.Null)
        return true;

    if (type == TileType.Tunnel)
        return true;

    return false;
}


void TunnelTopLeftCorner(int x, int y)
{
    //   checkOne = CheckSameTileAlternative(tile, x - 1, y + 1);
    //  checkTwo = CheckSameTileAlternative(tile, x, y + 1);
    TileType topLeft = CheckTileType(x - 1, y + 1);
    TileType top = CheckTileType(x, y + 1);
    TileType left = CheckTileType(x - 1, y);

    if(topLeft != top && left == TileType.Tunnel)
    {
        InnerTopLeftCorner(x, y);
        return;
    }

    if(topLeft != TileType.Tunnel && topLeft == top && topLeft == left)
    {
        InnerTopLeftCorner(x, y);
        return;
    }

    if(topLeft == TileType.Null && top == TileType.Null && !TunnelOrNull(left))
    {
        InnerTopLeftCorner(x, y);
        return;
    }

    if(left == TileType.Null && !TunnelOrNull(top) && !TunnelOrNull(topLeft))
    { 
        InnerTopLeftCorner(x, y);
        return;
    }

}

    if (typeOne == TileType.Null || typeTwo == TileType.Null)
        return;


    if (typeOne != typeTwo)
    {
        InnerTopLeftCorner(x, y);
        return;
    }
}


void GenericTopLeftCorner(int x, int y)
{
    bool checkOne;
    bool checkTwo;
    bool checkThree;
    TileMasterClass tile = tileArray[x, y];

    if (tileArray[x - 1, y] == null && tileArray[x, y + 1] == null)
    {
        TopLeftCorner(x, y);
    }

    checkOne = CheckNullOrDifferentTile(tile, x - 1, y);
    checkTwo = CheckNullOrDifferentTile(tile, x, y + 1);

    if (checkOne && checkTwo)
    {
        TopLeftCorner(x, y);
    }

    checkOne = CheckSameTile(tile, x - 1, y);
    checkTwo = CheckSameTile(tile, x, y + 1);
    checkThree = CheckNullOrDifferentTile(tileArray[x, y], x - 1, y + 1);

    if (checkOne && checkTwo && checkThree)
    {
        TopLeftCorner(x, y);
    }
}

void TopLeftCorner(int x, int y)
{
    Vector3 calculate;
    PoolChild poolChild;

    calculate = tileArray[x, y].transform.position;
    calculate.x -= 0.85f;
    calculate.y += 0.5f;
    calculate.z += 0.85f;

    poolChild = tileArray[x, y].majorColumnPool.RequestObject();
    poolChild.transform.position = calculate;
    contents.Add(poolChild);
}

void InnerTopLeftCorner(int x, int y)
{
    Vector3 calculate;
    PoolChild poolChild;

    calculate = tileArray[x, y].transform.position;
    calculate.x -= 1;
    calculate.y += 0.5f;
    calculate.z += 1;

    poolChild = tileArray[x, y].majorColumnPool.RequestObject();
    poolChild.transform.position = calculate;
    contents.Add(poolChild);
}



void GenerateTopRightCorner(int x, int y)
{
    Vector3 calculate;
    PoolChild poolChild;

  //  if (!CheckTile(tileArray[x, y], x, y + 1) && !CheckTile(tileArray[x, y], x + 1, y))
    {
        calculate = tileArray[x, y].transform.position;
        calculate.x -= 0.15f;
        calculate.y += 0.5f;
        calculate.z += 0.85f;

        poolChild = tileArray[x, y].majorColumnPool.RequestObject();
        poolChild.transform.position = calculate;
        contents.Add(poolChild);
        return;
    }

//    if (!CheckTile(tileArray[x, y], x, y + 1))
        return;

 //   if (!CheckTile(tileArray[x, y], x + 1, y))
        return;

//    if (CheckTile(tileArray[x, y], x + 1, y + 1))
        return;

    calculate = tileArray[x, y].transform.position;
    calculate.x -= 0.15f;
    calculate.y += 0.5f;
    calculate.z += 0.85f;

    poolChild = tileArray[x, y].majorColumnPool.RequestObject();
    poolChild.transform.position = calculate;
    contents.Add(poolChild);
}

void GenerateBottomLeftCorner(int x, int y)
{
    Vector3 calculate;
    PoolChild poolChild;

 //   if (!CheckTile(tileArray[x, y], x - 1, y) && !CheckTile(tileArray[x, y], x, y - 1))
    { 
        calculate = tileArray[x, y].transform.position;
        calculate.x -= 0.85f;
        calculate.y += 0.5f;
        calculate.z += 0.15f;

        poolChild = tileArray[x, y].majorColumnPool.RequestObject();
        poolChild.transform.position = calculate;
        contents.Add(poolChild);
        return;
    }

//    if (!CheckTile(tileArray[x, y], x, y - 1))
        return;

//    if (!CheckTile(tileArray[x, y], x - 1, y))
        return;

 //   if (CheckTile(tileArray[x, y], x - 1, y - 1))
        return;

    calculate = tileArray[x, y].transform.position;
    calculate.x -= 1;
    calculate.y += 0.5f;

    poolChild = tileArray[x, y].majorColumnPool.RequestObject();
    poolChild.transform.position = calculate;
    contents.Add(poolChild);
}

void GenerateBottomRightCorner(int x, int y)
{
    Vector3 calculate;
    PoolChild poolChild;

 //   if (!CheckTile(tileArray[x, y], x + 1, y) && !CheckTile(tileArray[x, y], x, y - 1))
    {
        calculate = tileArray[x, y].transform.position;
        calculate.x -= 0.15f;
        calculate.y += 0.5f;
        calculate.z += 0.15f;

        poolChild = tileArray[x, y].majorColumnPool.RequestObject();
        poolChild.transform.position = calculate;
        contents.Add(poolChild);
        return;
    }

 //   if (!CheckTile(tileArray[x, y], x, y - 1))
        return;

//     if (!CheckTile(tileArray[x, y], x + 1, y))
        return;

//      if (CheckTile(tileArray[x, y], x + 1, y - 1))
        return;

    calculate = tileArray[x, y].transform.position;
    calculate.y += 0.5f;

    poolChild = tileArray[x, y].majorColumnPool.RequestObject();
    poolChild.transform.position = calculate;
    contents.Add(poolChild);
} 

void GenerateTopColumn(int x, int y)
{
//       if (CheckTile(tileArray[x, y], x, y + 1))
        return;

//       if (!CheckTile(tileArray[x, y], x + 1, y))
        return;

//      if (CheckTile(tileArray[x, y], x + 1, y + 1))
        return;

    Vector3 calculate = tileArray[x, y].transform.position;
    calculate.y += 0.5f;
    calculate.z += 0.85f;

    PoolChild poolChild = tileArray[x, y].minorColumnPool.RequestObject();
    poolChild.transform.position = calculate;
    contents.Add(poolChild);
}

void GenerateBottomColumn(int x, int y)
{
//     if (CheckTile(tileArray[x, y], x, y - 1))
        return;

//       if (!CheckTile(tileArray[x, y], x + 1, y))
        return;

//       if (CheckTile(tileArray[x, y], x + 1, y - 1))
        return;

    Vector3 calculate = tileArray[x, y].transform.position;
    calculate.y += 0.5f;
    calculate.z += 0.15f;

    PoolChild poolChild = tileArray[x, y].minorColumnPool.RequestObject();
    poolChild.transform.position = calculate;
    contents.Add(poolChild);
}

void GenerateLeftColumn(int x, int y)
{
//       if (CheckTile(tileArray[x, y], x - 1, y))
        return;

//     if (!CheckTile(tileArray[x, y], x, y + 1))
        return;

//      if (CheckTile(tileArray[x, y], x - 1, y + 1))
        return;

    Vector3 calculate = tileArray[x, y].transform.position;
    calculate.x -= 0.85f;
    calculate.y += 0.5f;
    calculate.z += 1;

    PoolChild poolChild = tileArray[x, y].minorColumnPool.RequestObject();
    poolChild.transform.position = calculate;
    contents.Add(poolChild);
}

void GenerateRightColumn(int x, int y)
{
//     if (CheckTile(tileArray[x, y], x + 1, y))
        return;

//    if (!CheckTile(tileArray[x, y], x, y + 1))
        return;

 //   if (CheckTile(tileArray[x, y], x + 1, y + 1))
        return;

    Vector3 calculate = tileArray[x, y].transform.position;
    calculate.x -= 0.15f;
    calculate.y += 0.5f;
    calculate.z += 1;

    PoolChild poolChild = tileArray[x, y].minorColumnPool.RequestObject();
    poolChild.transform.position = calculate;
    contents.Add(poolChild);
}
}



*/