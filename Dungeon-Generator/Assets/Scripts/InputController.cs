using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class InputController : MonoBehaviour
{
    // script dedicated to take inputs + place temporary tiles
    // + place the selected tiles + instruct object array script to generate content

    // placed at raycast tile
   // [SerializeField] GameObject raycastPrefab;
    PoolChild raycastTile;

    // layer to raycast
    [SerializeField] LayerMask raycastMask;

    // core values to determine where to place tiles
    Vector3 initialRaycastPos;
    Vector3 currentRaycastPos;
    Vector3 previousRaycastPos;

    // temporary tiles stored in a list
   // List<PoolChild> currentlySelected = new List<PoolChild>();

    // for player to switch between different tyles
    int currentSelectedTilePool = 0;
    [SerializeField] List<ObjectPoolMasterclass> tilePools;

    private void Awake()
    {
        raycastTile = tilePools[currentSelectedTilePool].RequestObject();
        string currentPrefabName = tilePools[currentSelectedTilePool].GetPrefabName();
        UIManager.instance.UpdateText(currentPrefabName);
    }

    void Update()
    {
        ExecuteRaycast();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ActionSwitch();
        }
        else if (Input.GetMouseButtonDown(0))
        {
            MouseButtonDown();
        }
        else if (Input.GetMouseButton(0))
        {
            MouseButton();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            MouseButtonUp();
        }
        else
        {
            MouseInactive();
        }
    }

    void ExecuteRaycast()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, raycastMask))
        {
            Vector3 newVector = new Vector3(Mathf.RoundToInt(hit.point.x), 0, Mathf.RoundToInt(hit.point.z));
            newVector.x = Mathf.Clamp(newVector.x, 0, 50);
            newVector.z = Mathf.Clamp(newVector.z, 0, 50);
            currentRaycastPos = newVector;
        }
        else
        {
            currentRaycastPos.x = -1;
        }
    }

    void ActionSwitch()
    {
        raycastTile.ReturnChildToPool();
        currentSelectedTilePool++;
        if (currentSelectedTilePool > tilePools.Count - 1)
        {
            currentSelectedTilePool = 0;
        }

        string currentPrefabName = tilePools[currentSelectedTilePool].GetPrefabName();
        UIManager.instance.UpdateText(currentPrefabName);
        raycastTile = tilePools[currentSelectedTilePool].RequestObject();
    }

    void MouseButtonDown()
    {
        raycastTile.gameObject.SetActive(false);
        initialRaycastPos = currentRaycastPos;
    }

    void MouseButton()
    {
        if (currentRaycastPos == previousRaycastPos)
            return;

        bool destroy = currentSelectedTilePool == tilePools.Count-1;
        ObjectArray.instance.GenerateTemporaryArray(destroy, initialRaycastPos, currentRaycastPos, tilePools[currentSelectedTilePool]);

        previousRaycastPos = currentRaycastPos;
        GenerationManager.instance.Generate();

    }

    void MouseButtonUp()
    {
        raycastTile.gameObject.SetActive(true);
        ObjectArray.instance.FinaliseArray();
    }

    void MouseInactive()
    {
        if (currentRaycastPos.x == -1)
        {
            raycastTile.gameObject.SetActive(false);
        }
        else
        {
            raycastTile.gameObject.SetActive(true);
            Vector3 calcPos = currentRaycastPos;
            calcPos.y += 0.1f;
            raycastTile.transform.position = calcPos;
        }
    }
}









// ObjectArray.instance.DisableOjectInArray((int)currentTargetTile.x, (int)currentTargetTile.z);

/*
for(int i = 0; recentRaycastTiles.Count > i; i++)
{
    if (recentRaycastTiles[i] == currentTargetTile)
    {
        continue;
    }

    ObjectArray.instance.ActivateObjectInArray((int)recentRaycastTiles[i].x, (int)recentRaycastTiles[i].y);

}

recentRaycastTiles.Clear();

*/

/*
 * 
void PlaceNewTiles()
{
    // loop through all temporary tiles + place the chosen tiles + assign to 2d array
    for(int i = 0; i < currentlySelected.Count; i++)
    {
        PoolChild poolChild = tilePools[currentSelectedTilePool].RequestObject();
        Vector3 targetPosition = currentlySelected[i].transform.position;
        poolChild.transform.position = targetPosition;
        TileMasterClass tile = poolChild.GetComponent<TileMasterClass>();
     //   ObjectArray.instance.AssignObjectToArray(tile, (int)targetPosition.x, (int)targetPosition.z);
        currentlySelected[i].ReturnChildToPool();
    }

    currentlySelected.Clear();
}



void ReturnCurrentSelectedToPool()
{
    for (int i = 0; i < currentlySelected.Count; i++)
    {
        currentlySelected[i].ReturnChildToPool();
    }

    currentlySelected.Clear();
}


}



*         if (initialTile.x == currentTargetTile.x && initialTile.z == currentTargetTile.z)
    {
        PoolChild poolChild = TemporaryTilePool.instance.RequestObject();
        poolChild.transform.position = new Vector3(initialTile.x, 0, currentTargetTile.z);
        currentlySelected.Add(poolChild);
    }
    else if (initialTile.x > currentTargetTile.x && initialTile.z == currentTargetTile.z)
    {
        for (int x = (int)initialTile.x; x > currentTargetTile.x - 1; x--)
        {
            PoolChild poolChild = TemporaryTilePool.instance.RequestObject();
            poolChild.transform.position = new Vector3(x, 0, currentTargetTile.z);
            currentlySelected.Add(poolChild);
        }
    }
    else if (initialTile.x < currentTargetTile.x && initialTile.z == currentTargetTile.z)
    {
        for (int x = (int)initialTile.x; x < currentTargetTile.x + 1; x++)
        {
            PoolChild poolChild = TemporaryTilePool.instance.RequestObject();
            poolChild.transform.position = new Vector3(x, 0, currentTargetTile.z);
            currentlySelected.Add(poolChild);
        }
    }
    else if (initialTile.x == currentTargetTile.x && initialTile.z < currentTargetTile.z)
    {
        for (int z = (int)initialTile.z; z < currentTargetTile.z + 1; z++)
        {
            PoolChild poolChild = TemporaryTilePool.instance.RequestObject();
            poolChild.transform.position = new Vector3(initialTile.x, 0, z);
            currentlySelected.Add(poolChild);
        }
    }
    else if (initialTile.x == currentTargetTile.x && initialTile.z > currentTargetTile.z)
    {
        for (int z = (int)initialTile.z; z > currentTargetTile.z - 1; z--)
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
            for (int z = (int)initialTile.z; z < currentTargetTile.z + 1; z++)
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
*/