using UnityEngine;

public class CameraController : MonoBehaviour
{
    public CameraType cameraType = CameraType.EagleView;

    [Header("Projection")]
    public ProjectionMode projectionMode = ProjectionMode.Perspective;
    public float orthographicSize = 10;

    [Header("Common Settings")]
    public Transform target;
    public float distance = 10f;
    public float height = 5f;
    public float pitch = 30f;
    public float yaw = 0f;
    public float fov = 60f;
    public Vector3 offset;

    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
            cam = Camera.main;
    }

    private void LateUpdate()
    {
        if (!target) return;

        switch (cameraType)
        {
            case CameraType.EagleView:
                ApplyEagleView();
                break;
            case CameraType.Isometric:
                ApplyIsometricView();
                break;
            case CameraType.TopDown:
                ApplyTopDownView();
                break;
            case CameraType.ThirdPerson:
                ApplyThirdPersonView();
                break;
            case CameraType.FreeCamera:
                ApplyFreeCamera();
                break;
        }

        if (cam != null)
        {
            switch (projectionMode)
            {
                case ProjectionMode.Perspective:
                    cam.orthographic = false;
                    break;
                case ProjectionMode.Orthographic:
                    cam.orthographic = true;
                    cam.orthographicSize = orthographicSize;
                    break;
            }
        }

        cam.fieldOfView = fov;
    }

    private void ApplyEagleView()
    {
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 dir = rotation * Vector3.back;
        transform.position = target.position + dir * distance + Vector3.up * height + offset;
        transform.LookAt(target);
    }

    private void ApplyFreeCamera()
    {
        float moveSpeed = 10f;
        float rotSpeed = 120f;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float upDown = 0f;

        if (Input.GetKey(KeyCode.E)) upDown += 1f;
        if (Input.GetKey(KeyCode.Q)) upDown -= 1f;

        Vector3 dir = new Vector3(h, upDown, v);
        transform.position += transform.TransformDirection(dir) * moveSpeed * Time.deltaTime;

        if (Input.GetMouseButton(1)) // Right mouse button = rotate
        {
            float mouseX = Input.GetAxis("Mouse X") * rotSpeed * Time.deltaTime;
            float mouseY = -Input.GetAxis("Mouse Y") * rotSpeed * Time.deltaTime;
            transform.eulerAngles += new Vector3(mouseY, mouseX, 0f);
        }
    }

    private void ApplyIsometricView()
    {
        transform.position = target.position + new Vector3(-distance, height, -distance) + offset;
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    private void ApplyTopDownView()
    {
        transform.position = target.position + Vector3.up * height + offset;
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }

    private void ApplyThirdPersonView()
    {
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 dir = rotation * Vector3.back;
        transform.position = target.position + dir * distance + offset;
        transform.LookAt(target);
    }
}
public enum CameraType
{
    Isometric,
    TopDown,
    EagleView,
    ThirdPerson,
    FreeCamera
}

public enum ProjectionMode
{
    Perspective,
    Orthographic
}