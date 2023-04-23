using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    public float speed = 10f; // Speed of camera movement
    public float sensitivity = 3f; // Sensitivity of mouse movement

    void Update()
    {

        // Move camera on button press
        if (Input.GetKeyDown(KeyCode.W))
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
        else
        {
            float horizontal = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
            float vertical = Input.GetAxis("Vertical") * speed * Time.deltaTime;
            transform.Translate(horizontal, 0, vertical);
        }

        // Rotate camera
        float rotateHorizontal = Input.GetAxis("Mouse X") * sensitivity;
        float rotateVertical = Input.GetAxis("Mouse Y") * sensitivity;
        transform.Rotate(Vector3.up, rotateHorizontal, Space.World);
        transform.Rotate(Vector3.left, rotateVertical, Space.Self);
    }
}
