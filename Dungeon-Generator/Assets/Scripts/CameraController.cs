using UnityEngine;

public class CameraController : MonoBehaviour
{
    private void Update()
    {
        Vector3 _currentPos = transform.position;

        if(Input.GetKey(KeyCode.A))
        {
            _currentPos.x -= 10f * Time.deltaTime;
        }
        else if(Input.GetKey(KeyCode.D))
        {
            _currentPos.x += 10f * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.W))
        {
            _currentPos.z += 10f * Time.deltaTime;
        }
        else if(Input.GetKey(KeyCode.S))
        {
            _currentPos.z -= 10f * Time.deltaTime;
        }

        transform.position = _currentPos;
    }
}
