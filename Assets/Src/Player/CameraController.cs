using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float panSpeed = 20f;
    public float zoomSpeed = 5f;

    void Update()
    {
        // Move the camera in WASD directions
        MoveCamera();

        // Pan the camera while holding space and moving the mouse
        PanCamera();

        // Zoom in and out using the mouse scroll wheel
        ZoomCamera();

    }

    void MoveCamera()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;
        Vector3 moveAmount = moveDirection * moveSpeed * Time.deltaTime;

        transform.Translate(moveAmount, Space.World);
    }

    void PanCamera()
    {
        if (Input.GetMouseButton(2))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            Vector3 panDirection = new Vector3(-mouseX, 0f, -mouseY).normalized;
            Vector3 panAmount = panDirection * panSpeed * Time.deltaTime;

            transform.Translate(panAmount, Space.World);
        }
    }

    void ZoomCamera()
    {
        float scrollWheel = Input.GetAxis("Mouse ScrollWheel");

        Vector3 zoomAmount = new Vector3(0f, 0f, scrollWheel * zoomSpeed);
        transform.Translate(zoomAmount, Space.Self);
    }
}
