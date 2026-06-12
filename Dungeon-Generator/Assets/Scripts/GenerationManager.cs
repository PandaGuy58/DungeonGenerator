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

    public void RegenerateTiles()
    {
        GenerationData[,] array = ObjectArray.instance.RequestTemporaryArray();
        ReturnToPool(generatedTiles);
        GenerateTiles(array);
    }

    public void DisableContents()
    {
        ReturnToPool(contents);
    }

    public void GenerateContents()
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

             //   GenerateTopLeftCorner(x, z);
            //    GenerateTopRightCorner(x, z);
            //    GenerateBottomLeftCorner(x, z);
            //    GenerateBottomRightCorner(x, z);

           //     GenerateTopColumn(x, z);
           //     GenerateBottomColumn(x, z);
            //    GenerateLeftColumn(x, z);
            //    GenerateRightColumn(x, z);
            }
        }
    }

    void ReturnToPool(List<PoolChild> poolChildList)
    {
        for (int i = 0; i < poolChildList.Count; i++)
        {
            poolChildList[i].ReturnChildToPool();
        }
        poolChildList.Clear();
    }

    void GenerateTiles(GenerationData[,] dataArray)
    {
        tileArray = new TileMasterClass[51, 51];
        for (int x = 0; x < dataArray.GetLength(0); x++)
        {
            for (int z = 0; z < dataArray.GetLength(1); z++)
            {
                if (dataArray[x, z] == null)
                    continue;

                PoolChild newTile = dataArray[x, z].pool.RequestObject();
                newTile.transform.position = new Vector3(x + 0.5f, 0.1f, z - 0.5f);
                generatedTiles.Add(newTile);

                if (newTile.IsDestructive())
                    continue;

                TileMasterClass tile = newTile.GetComponent<TileMasterClass>();

                tileArray[x, z] = tile;

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






    /* 

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
    */


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
        /*
        if (typeOne == TileType.Null || typeTwo == TileType.Null)
            return;


        if (typeOne != typeTwo)
        {
            InnerTopLeftCorner(x, y);
            return;
        }
    }
        */

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



