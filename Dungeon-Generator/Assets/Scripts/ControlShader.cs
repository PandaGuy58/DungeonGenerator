using UnityEngine;

public class ControlShader : MonoBehaviour
{
    TileMasterClass tileMasterClass;

    [SerializeField] float x;

    [SerializeField] float y;

    private void Awake()
    {
        tileMasterClass = GetComponent<TileMasterClass>();
    }

    private void Update()
    {
      //  tileMasterClass.SetOffset(x, y);
    }
}
