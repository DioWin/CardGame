using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text label;
    [SerializeField] private RectTransform visual;

    [SerializeField] private float draggSpeed = 20f;
    [Header("Rotation")]
    [SerializeField] private float rotationMultiplier = 1.2f;
    [SerializeField] private float maxTiltAngle = 10f;
    [SerializeField] private float tiltSmoothTime = 0.08f;
    [SerializeField] private float resetToDefaultDuration = 0.2f;
    [SerializeField] private Canvas canvas;

    [Header("Nudge Settings")]
    [SerializeField] private float nudgeAngle = 10f;
    [SerializeField] private float nudgeDuration = 0.08f;
    [SerializeField] private float returnDuration = 0.12f;

    private Transform modelRect;
    private Vector2 lastMousePosition;
    private float currentTilt;
    private float tiltVelocity;

    public bool isDragging;
    bool nudgeInProgress;
    bool isInitialized;

    [SerializeField] private CanvasGroup canvasGroup;

    public void SetData(CardModel model, Transform modelRect, Transform visualContainer)
    {
        label.text = model.cardName;
        icon.sprite = model.icon;

        this.modelRect = modelRect;
        isInitialized = true;

        visual.SetParent(visualContainer);
        visual.transform.position = transform.position;
    }

    private void Update()
    {
        if (!isInitialized)
            return;

        if (isDragging)
        {
            FallowCursor();
            HandleRotation();
        }
        else
        {
            visual.position = Vector3.Lerp(visual.position, modelRect.position, Time.deltaTime * 10f);

            if (!nudgeInProgress)
                visual.localRotation = Quaternion.Euler(0f, 0f, modelRect.eulerAngles.z);
        }
    }

    private void FallowCursor()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            visual.parent as RectTransform,
            Input.mousePosition,
            Camera.main,
            out Vector2 localPoint
        );

        visual.anchoredPosition = Vector2.Lerp(visual.anchoredPosition, localPoint, Time.deltaTime * draggSpeed);
    }

    public void SetAlpha(float alpha)
    {
        canvasGroup.alpha = alpha;
        canvasGroup.blocksRaycasts = alpha > 0.1f;
    }

    private void HandleRotation()
    {
        Vector2 mouseDelta = (Vector2)Input.mousePosition - lastMousePosition;
        lastMousePosition = Input.mousePosition;

        float targetTilt = Mathf.Clamp(-mouseDelta.x * rotationMultiplier, -maxTiltAngle, maxTiltAngle);

        currentTilt = Mathf.SmoothDamp(currentTilt, targetTilt, ref tiltVelocity, tiltSmoothTime);

        visual.localRotation = Quaternion.Euler(0f, 0f, currentTilt);
    }

    public void NudgeFromDirection(float direction)
    {
        nudgeInProgress = true;
        float angle = direction > 0 ? -nudgeAngle : nudgeAngle;

        visual.DOLocalRotate(new Vector3(0f, 0f, angle), nudgeDuration)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                visual.DOLocalRotate(Vector3.zero, returnDuration).SetEase(Ease.OutCubic).OnComplete(() =>
                {
                    nudgeInProgress = false;
                });
            });
    }

    public void SetDraggStatus(bool value)
    {
        isDragging = value;

        if (value)
        {
            canvas.overrideSorting = true;
            canvas.sortingOrder = 2;

            lastMousePosition = Input.mousePosition;
            SetAlpha(1f);
        }
        else
        {
            canvas.overrideSorting = false;
            SetAlpha(1f);
        }
    }

    public void Reset()
    {
        visual.DOLocalRotate(Vector3.zero, resetToDefaultDuration).SetEase(Ease.OutQuad);
    }

    public RectTransform GetVisual()
    {
        return visual;
    }
}
