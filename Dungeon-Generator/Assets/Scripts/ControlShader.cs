using UnityEngine;

public class ControlShader : MonoBehaviour
{
    private Renderer _rend;

    private void Awake()
    {
        _rend = GetComponent<Renderer>();
    }

    public void Activate(bool active)
    {
        if (active)
        {
            _rend.material.SetFloat("_Active", 1);
        }
        else
        {
            _rend.material.SetFloat("_Active", 0);
        }
    }
}
