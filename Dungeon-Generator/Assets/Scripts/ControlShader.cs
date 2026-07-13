using UnityEngine;

public class ControlShader : MonoBehaviour
{
    Renderer rend;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    public void Activate(bool active)
    {
        if (active)
        {
            rend.material.SetFloat("_Active", 1);
        }
        else
        {
            rend.material.SetFloat("_Active", 0);
        }
    }
}
