using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class InputController : MonoBehaviour
{
    [SerializeField] GameObject raycastPrferab;

    [SerializeField] LayerMask raycastMask;

    [SerializeField] private Vector3 currentRaycastCoordinate;
    GameObject raycastTile;

    //bool raycastSuccess = false;
    [SerializeField] private Vector3 initialTile;
    [SerializeField] private Vector3 currentTargetTile;
    [SerializeField] private Vector3 previousTargetTile;

    List<PoolChild> currentlySelected = new List<PoolChild>();

    int currentSelectedTilePool = 0;
    [SerializeField] List<ObjectPoolMasterclass> tilePools;


    private void Awake()
    {
        raycastTile = Instantiate(raycastPrferab, Vector3.zero,quaternion.identity);
        raycastTile.SetActive(false);

        string currentPrefabName = tilePools[currentSelectedTilePool].GetPrefabName();
        UIManager.instance.UpdateText(currentPrefabName);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            currentSelectedTilePool++;
            if(currentSelectedTilePool > tilePools.Count-1)
            {
                currentSelectedTilePool = 0;
            }

            string currentPrefabName = tilePools[currentSelectedTilePool].GetPrefabName();
            UIManager.instance.UpdateText(currentPrefabName);
        }

        ExecuteRaycast();

        if (Input.GetMouseButtonDown(0))
        {
            MouseButtonDown();
        }
        else if(Input.GetMouseButton(0))
        {
            MouseButton();
        }
        else if(Input.GetMouseButtonUp(0))
        {
            PlaceNewTiles();
            raycastTile.SetActive(true);
            ObjectArray.instance.GenerateContent();
        }
        else
        {
            if(currentRaycastCoordinate.x == -1)
            {
                
                raycastTile.SetActive(false);
            }
            else
            {
                raycastTile.SetActive(true);
                raycastTile.transform.position = currentRaycastCoordinate;
            }
        }
    }

    void PlaceNewTiles()
    {
        for(int i = 0; i < currentlySelected.Count; i++)
        {
            PoolChild poolChild = tilePools[currentSelectedTilePool].RequestObject();
            Vector3 targetPosition = currentlySelected[i].transform.position;
            poolChild.transform.position = targetPosition;
            TileMasterClass tile = poolChild.GetComponent<TileMasterClass>();
            ObjectArray.instance.AssignObjectToArray(tile, (int)targetPosition.x, (int)targetPosition.z);
            currentlySelected[i].ReturnChildToPool();
        }

        currentlySelected.Clear();
    }

    void ExecuteRaycast()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, raycastMask))
        {
            Vector3 newVector = new Vector3(Mathf.RoundToInt(hit.point.x), 0, Mathf.RoundToInt(hit.point.z));
            newVector.x = Mathf.Clamp(newVector.x, 0, 50);
            newVector.z = Mathf.Clamp(newVector.z, 0, 50);
            currentRaycastCoordinate = newVector;
        }
        else
        {
            currentRaycastCoordinate.x = -1;
        }
    }

    void ReturnCurrentSelectedToPool()
    {
        for (int i = 0; i < currentlySelected.Count; i++)
        {
            currentlySelected[i].ReturnChildToPool();
        }

        currentlySelected.Clear();
    }

    void MouseButtonDown()
    {
        if (currentRaycastCoordinate == Vector3.negativeInfinity)
            return;

        raycastTile.SetActive(false);
        initialTile = currentRaycastCoordinate;

        /*
        for (int i = 0; i < currentlySelected.Count; i++)
        {
            Vector3 targetLocation = currentlySelected[i].transform.position;
            PoolChild poolChild = tilePools[currentSelectedTilePool].RequestObject();
            poolChild.transform.position = targetLocation;
        }
        */
    }

    void MouseButton()
    {
        currentTargetTile = currentRaycastCoordinate;

        if (currentTargetTile == previousTargetTile)
            return;

        previousTargetTile = currentTargetTile;
        ReturnCurrentSelectedToPool();

        if (initialTile.x == currentTargetTile.x && initialTile.z == currentTargetTile.z)
        {
            PoolChild poolChild = TemporaryTilePool.instance.RequestObject();
            poolChild.transform.position = new Vector3(initialTile.x, 0, currentTargetTile.z);
            currentlySelected.Add(poolChild);
        }
        else if (initialTile.x > currentTargetTile.x && initialTile.z == currentTargetTile.z)
        {
            for (int x = (int)initialTile.x; x > currentTargetTile.x -1; x--)
            {
                PoolChild poolChild = TemporaryTilePool.instance.RequestObject();
                poolChild.transform.position = new Vector3(x, 0, currentTargetTile.z);
                currentlySelected.Add(poolChild);
            }
        }
        else if (initialTile.x < currentTargetTile.x && initialTile.z == currentTargetTile.z)
        {
            for (int x = (int)initialTile.x; x < currentTargetTile.x +1; x++)
            {
                PoolChild poolChild = TemporaryTilePool.instance.RequestObject();
                poolChild.transform.position = new Vector3(x, 0, currentTargetTile.z);
                currentlySelected.Add(poolChild);
            }
        }
        else if (initialTile.x == currentTargetTile.x && initialTile.z < currentTargetTile.z)
        {
            for (int z = (int)initialTile.z; z < currentTargetTile.z +1; z++)
            {
                PoolChild poolChild = TemporaryTilePool.instance.RequestObject();
                poolChild.transform.position = new Vector3(initialTile.x, 0, z);
                currentlySelected.Add(poolChild);
            }
        }
        else if (initialTile.x == currentTargetTile.x && initialTile.z > currentTargetTile.z)
        {
            for (int z = (int)initialTile.z; z > currentTargetTile.z -1; z--)
            {
                PoolChild poolChild = TemporaryTilePool.instance.RequestObject();
                poolChild.transform.position = new Vector3(initialTile.x, 0, z);
                currentlySelected.Add(poolChild);
            }
        }
        else if (initialTile.x < currentTargetTile.x && initialTile.z < currentTargetTile.z)
        {
            for (int x = (int)initialTile.x; x < currentTargetTile.x + 1; x++)
            {
                for (int z = (int)initialTile.z; z < currentTargetTile.z +1; z++)
                {
                    PoolChild poolChild = TemporaryTilePool.instance.RequestObject();
                    poolChild.transform.position = new Vector3(x, 0, z);
                    currentlySelected.Add(poolChild);
                }
            }
        }
        else if (initialTile.x > currentTargetTile.x && initialTile.z < currentTargetTile.z)
        {
            for (int x = (int)initialTile.x; x > currentTargetTile.x - 1; x--)
            {
                for (int z = (int)initialTile.z; z < currentTargetTile.z + 1; z++)
                {
                    PoolChild poolChild = TemporaryTilePool.instance.RequestObject();
                    poolChild.transform.position = new Vector3(x, 0, z);
                    currentlySelected.Add(poolChild);
                }
            }
        }
        else if (initialTile.x < currentTargetTile.x && initialTile.z > currentTargetTile.z)
        {
            for (int x = (int)initialTile.x; x < currentTargetTile.x + 1; x++)
            {
                for (int z = (int)initialTile.z; z > currentTargetTile.z - 1; z--)
                {
                    PoolChild poolChild = TemporaryTilePool.instance.RequestObject();
                    poolChild.transform.position = new Vector3(x, 0, z);
                    currentlySelected.Add(poolChild);
                }
            }
        }
        else if (initialTile.x > currentTargetTile.x && initialTile.z > currentTargetTile.z)
        {
            for (int x = (int)initialTile.x; x > currentTargetTile.x - 1; x--)
            {
                for (int z = (int)initialTile.z; z > currentTargetTile.z - 1; z--)
                {
                    PoolChild poolChild = TemporaryTilePool.instance.RequestObject();
                    poolChild.transform.position = new Vector3(x, 0, z);
                    currentlySelected.Add(poolChild);
                }
            }
        }
    }
}
        

