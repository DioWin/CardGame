using UnityEngine;
using System.Collections.Generic;

public class MouseVelocityTracker : MonoBehaviour
{
    private CardController cardController;

    [Header("Mouse Velocity Settings")]
    public int velocitySampleCount = 5;
    public int baseTrackingDistance = 20;
    public int trackingMultiplier = 2;

    private Queue<Vector3> mouseWorldPositions = new Queue<Vector3>();
    public Vector3 AveragedVelocity { get; private set; }

    private Camera mainCamera;
    private bool isDrugging;

    private void Awake()
    {
        mainCamera = Camera.main;
        cardController = GetComponent<CardController>();

        cardController.OnDragStatusChangedEvent += OnDragStatusChanged;
    }

    private void OnDestroy()
    {
        cardController.OnDragStatusChangedEvent -= OnDragStatusChanged;
    }

    private void OnDragStatusChanged(bool status)
    {
        isDrugging = status;
    }

    public void ResetTracking()
    {
        mouseWorldPositions.Clear();
        AveragedVelocity = Vector3.zero;
    }

    public void Update()
    {
        if(isDrugging)
        {
            Track();
        }
    }

    public void Track()
    {
        float mouseY01 = Mathf.Clamp01(Input.mousePosition.y / Screen.height);
        float dynamicDistance = baseTrackingDistance * Mathf.Lerp(1f, trackingMultiplier, mouseY01);

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Vector3 planeOrigin = mainCamera.transform.position + mainCamera.transform.forward * dynamicDistance;
        Plane movementPlane = new Plane(-mainCamera.transform.forward, planeOrigin);

        if (movementPlane.Raycast(ray, out float enter))
        {
            Vector3 worldPoint = ray.GetPoint(enter);
            mouseWorldPositions.Enqueue(worldPoint);

            if (mouseWorldPositions.Count > velocitySampleCount)
                mouseWorldPositions.Dequeue();

            if (mouseWorldPositions.Count >= 2)
            {
                Vector3 first = mouseWorldPositions.Peek();
                Vector3 last = worldPoint;
                float totalTime = Time.deltaTime * (mouseWorldPositions.Count - 1);
                AveragedVelocity = (last - first) / totalTime;
            }
        }
    }
}
