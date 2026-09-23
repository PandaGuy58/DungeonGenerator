using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    // script dedicated to take inputs + place temporary tiles
    // + place the selected tiles + instruct object array script to generate content

    // layer to raycast
    [SerializeField] private LayerMask _raycastMask;

    // core values to determine where to place tiles
    private Vector3 _initialRaycastPos;
    private Vector3 _currentRaycastPos;
    private Vector3 _previousRaycastPos;

    // for player to switch between different biomes
    private int _selectedBiome = 0;
    [SerializeField] private List<Biome> _biomes;


    private void Awake()
    {
        UIManager._instance.UpdateText(_biomes[_selectedBiome].name);
    }

    void Update()
    {
        ExecuteRaycast();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ActionSwitch();
            ExecuteTileGeneration(_currentRaycastPos, _currentRaycastPos);
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
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, _raycastMask))
        {
            Vector3 newVector = new Vector3(Mathf.RoundToInt(hit.point.x), 0, Mathf.RoundToInt(hit.point.z));
            newVector.x = Mathf.Clamp(newVector.x, 1, 49);
            newVector.z = Mathf.Clamp(newVector.z, 1, 49);
            _currentRaycastPos = newVector;
        }
        else
        {
            _currentRaycastPos.x = -1;
        }
    }

    void ActionSwitch()
    {
        _selectedBiome++;
        if (_selectedBiome > _biomes.Count - 1)
        {
            _selectedBiome = 0;
        }

        UIManager._instance.UpdateText(_biomes[_selectedBiome].name);
    }

    void MouseButtonDown()
    {
        _initialRaycastPos = _currentRaycastPos;
        GenerationManager._instance.DestroyContents();
    }

    void MouseButton()
    {
        if (_currentRaycastPos == _previousRaycastPos)
            return;

        ExecuteTileGeneration(_initialRaycastPos, _currentRaycastPos);
    }

    void MouseButtonUp()
    {
        if (_biomes[_selectedBiome].IsDestructive())
        {
            ObjectArray.instance.RemoveFromArray();
            GenerationManager._instance.RegenerateTiles();
        }

        ObjectArray.instance.FinaliseArray();
        GenerationManager._instance.GenerateContents();
    }

    void MouseInactive()
    {
        if (_currentRaycastPos.x == -1)
            return;

        if (_currentRaycastPos == _previousRaycastPos)
            return;

        ExecuteTileGeneration(_currentRaycastPos, _currentRaycastPos);
    }

    void ExecuteTileGeneration(Vector3 initialTile, Vector3 currentTargetTile)
    {
        EventBus<ExecuteTempArrayGenerationEvent>.Publish(new ExecuteTempArrayGenerationEvent
        {
            currentTargetTile = currentTargetTile,
            initialTile = initialTile,
            biome = _biomes[_selectedBiome]
        });
        
        GenerationManager._instance.RegenerateTiles();
        _previousRaycastPos = _currentRaycastPos;
    }

}