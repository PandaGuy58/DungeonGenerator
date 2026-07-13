using UnityEngine;

public class ControlContent : MonoBehaviour
{
    [SerializeField] MeshRenderer render;
    float visibilityValue = 0;
    bool visible = true;
    
    public void Dissolve()
    {
        visible = false;
    }
    private void Update()
    {
        if (visible)
        {
            visibilityValue += Time.deltaTime * 3;
            if(visibilityValue >= 20)
            {
                enabled = false;
                visibilityValue = 20;
            }
        }
        else
        {
            visibilityValue -= Time.deltaTime * 3;
            if(visibilityValue <= 0)
            {
                enabled = false;
                visibilityValue = 0;
            }
        }
            
    }
}
