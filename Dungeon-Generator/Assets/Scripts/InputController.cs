using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    // script dedicated to take inputs + place temporary tiles
    // + place the selected tiles + instruct object array script to generate content

    // layer to raycast
    [SerializeField] LayerMask raycastMask;

    // core values to determine where to place tiles
    Vector3 initialRaycastPos;
    Vector3 currentRaycastPos;
    Vector3 previousRaycastPos;

    // for player to switch between different tyles
    int currentSelectedTilePool = 0;
    [SerializeField] List<ObjectPoolMasterclass> tilePools;

    private void Awake()
    {
        string currentPrefabName = tilePools[currentSelectedTilePool].GetPrefabName();
        UIManager.instance.UpdateText(currentPrefabName);
    }

    void Update()
    {
        ExecuteRaycast();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ActionSwitch();
            ExecuteTileGeneration(currentRaycastPos, currentRaycastPos);
        }
        else if(Input.GetMouseButtonDown(0))
        {
            MouseButtonDown();
        }
        else if (Input.GetMouseButton(0))
        {
            MouseButton();
        }
        else if(Input.GetMouseButtonUp(0))
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
            newVector.x = Mathf.Clamp(newVector.x, 1, 49);
            newVector.z = Mathf.Clamp(newVector.z, 1, 49);
            currentRaycastPos = newVector;
        }
        else
        {
            currentRaycastPos.x = -1;
        }
    }

    void ActionSwitch()
    {
        currentSelectedTilePool++;
        if (currentSelectedTilePool > tilePools.Count - 1)
        {
            currentSelectedTilePool = 0;
        }

        string currentPrefabName = tilePools[currentSelectedTilePool].GetPrefabName();
        UIManager.instance.UpdateText(currentPrefabName);
    }

    void MouseButtonDown()
    {
        initialRaycastPos = currentRaycastPos;
        GenerationManager.instance.DisableContents();
    }

    void MouseButton()
    {
        if (currentRaycastPos == previousRaycastPos)
            return;

        ExecuteTileGeneration(initialRaycastPos, currentRaycastPos);

    }

    void MouseButtonUp()  
    {
        if (currentSelectedTilePool == tilePools.Count - 1)
        {
            ObjectArray.instance.RemoveFromArray();
            GenerationManager.instance.RegenerateTiles();
        }

        ObjectArray.instance.FinalisePoolArray();
        GenerationManager.instance.GenerateContents();
    }

    void MouseInactive()
    {
        if (currentRaycastPos.x == -1)
            return;

        if (currentRaycastPos == previousRaycastPos)
            return;

        ExecuteTileGeneration(currentRaycastPos, currentRaycastPos);
    }

    void ExecuteTileGeneration(Vector3 initialTile, Vector3 currentTargetTile)
    {
        if (currentSelectedTilePool == tilePools.Count - 1)
        {
            ObjectArray.instance.GenerateTemporaryArrayDestruction(initialTile, currentTargetTile, tilePools[currentSelectedTilePool]);
        }
        else
        {
            ObjectArray.instance.GenerateTemporaryArray(initialTile, currentTargetTile, tilePools[currentSelectedTilePool]);
        }

        GenerationManager.instance.RegenerateTiles();
        previousRaycastPos = currentRaycastPos;
    }
}
