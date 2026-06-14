using System.Collections.Generic;
using UnityEngine;

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
        GenerateTunnelColumns();
        GenerateTileSplits();
        GenerateInnerCorners();
        GenerateOutsideCorners();
        GenerateColumns();
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
