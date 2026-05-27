using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private void Update()
    {
        Vector3 currentPos = transform.position;

        if(Input.GetKey(KeyCode.A))
        {
            currentPos.x -= 10f * Time.deltaTime;
        }
        else if(Input.GetKey(KeyCode.D))
        {
            currentPos.x += 10f * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.W))
        {
            currentPos.z += 10f * Time.deltaTime;
        }
        else if(Input.GetKey(KeyCode.S))
        {
            currentPos.z -= 10f * Time.deltaTime;
        }

        transform.position = currentPos;
    }
}
