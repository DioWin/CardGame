using UnityEngine;
using UnityEngine.EventSystems;

public class CardTiltAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Idle Tilt")]
    public float idleAmplitude = 2f;
    public float idleSpeed = 2f;

    [Header("Mouse Hover Tilt")]
    public float hoverTiltAmount = 10f;
    public float hoverLerpSpeed = 10f;

    [SerializeField] private RectTransform rectTransform;

    private bool isHovering;
    private float indexOffset;
    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;

        // Slight variation between cards
        indexOffset = Random.Range(0f, 10f);

        // Initial random tilt
        float startX = Mathf.Sin(indexOffset) * idleAmplitude;
        float startY = Mathf.Cos(indexOffset) * idleAmplitude;
        rectTransform.localRotation = Quaternion.Euler(startX, startY, 0f);
    }

    private void Update()
    {
        if (!isHovering)
        {
            // Idle waving motion
            float sine = Mathf.Sin(Time.time * idleSpeed + indexOffset);
            float cosine = Mathf.Cos(Time.time * idleSpeed + indexOffset);

            float idleX = sine * idleAmplitude;
            float idleY = cosine * idleAmplitude;

            Vector3 targetRotation = new Vector3(idleX, idleY, 0f);
            rectTransform.localRotation = Quaternion.Lerp(
                rectTransform.localRotation,
                Quaternion.Euler(targetRotation),
                Time.deltaTime * 5f
            );
        }
        else
        {
            // Mouse-based tilt relative to card center
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform,
                Input.mousePosition,
                cam,
                out Vector2 localPoint
            );

            Vector2 size = rectTransform.rect.size;
            Vector2 normalized = new Vector2(
                Mathf.Clamp(localPoint.x / (size.x * 0.5f), -1f, 1f),
                Mathf.Clamp(localPoint.y / (size.y * 0.5f), -1f, 1f)
            );

            float tiltX = normalized.y * hoverTiltAmount;
            float tiltY = -normalized.x * hoverTiltAmount;

            Vector3 targetRotation = new Vector3(tiltX, tiltY, 0f);
            rectTransform.localRotation = Quaternion.Lerp(
                rectTransform.localRotation,
                Quaternion.Euler(targetRotation),
                Time.deltaTime * hoverLerpSpeed
            );
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }
}
