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
    [SerializeField] private Canvas canvas;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Drag Settings")]
    [SerializeField] private float dragSpeed = 20f;
    [SerializeField] private float followDefaultPosSpeed = 20f;

    [Header("Rotation")]
    [SerializeField] private float rotationMultiplier = 1.2f;
    [SerializeField] private float maxTiltAngle = 10f;
    [SerializeField] private float tiltSmoothTime = 0.08f;
    [SerializeField] private float resetToDefaultDuration = 0.2f;

    [Header("Nudge Settings")]
    [SerializeField] private float nudgeAngle = 10f;
    [SerializeField] private float nudgeDuration = 0.08f;
    [SerializeField] private float returnDuration = 0.12f;

    [Header("Visual FX")]
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float animDuration = 0.15f;

    private Transform modelRect;
    private Vector2 lastMousePosition;
    private float currentTilt;
    private float tiltVelocity;

    public bool isDragging;
    private bool nudgeInProgress;
    private bool isInitialized;

    private string labelTweenId => $"LabelColor_{GetInstanceID()}";
    private string nudgeTweenId => $"Nudge_{GetInstanceID()}";
    private string rotateTweenId => $"Rotate_{GetInstanceID()}";

    public void SetData(CardModel model, Transform modelRect, Transform visualContainer)
    {
        label.text = model.cardName;

        if (model.icon != null)
            icon.sprite = model.icon;
        else
            icon.gameObject.SetActive(false);

        this.modelRect = modelRect;
        isInitialized = true;

        visual.SetParent(visualContainer);
        visual.transform.position = transform.position;

        canvas.overrideSorting = false;
        canvas.sortingOrder = transform.GetSiblingIndex();
    }

    private void Update()
    {
        if (!isInitialized)
            return;

        if (isDragging)
        {
            FollowCursor();
            HandleRotationToMouse();
        }
        else
        {
            visual.position = Vector3.Lerp(visual.position, modelRect.position, Time.deltaTime * followDefaultPosSpeed);

            if (!nudgeInProgress)
                LerpToRotation(modelRect.eulerAngles.z, 10f);
        }
    }

    private void FollowCursor()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            visual.parent as RectTransform,
            Input.mousePosition,
            Camera.main,
            out Vector2 localPoint
        );

        visual.anchoredPosition = Vector2.Lerp(visual.anchoredPosition, localPoint, Time.deltaTime * dragSpeed);
    }

    public void SetAlpha(float alpha)
    {
        canvasGroup.alpha = alpha;
        canvasGroup.blocksRaycasts = alpha > 0.1f;
    }

    private void LerpToRotation(float targetZ, float lerpSpeed)
    {
        float currentZ = visual.localEulerAngles.z;

        if (targetZ - currentZ > 180f) currentZ += 360f;
        else if (currentZ - targetZ > 180f) targetZ += 360f;

        float lerpedZ = Mathf.Lerp(currentZ, targetZ, Time.deltaTime * lerpSpeed);
        visual.localRotation = Quaternion.Euler(0f, 0f, lerpedZ);
    }

    private void HandleRotationToMouse()
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

        DOTween.Kill(nudgeTweenId);
        visual.DOLocalRotate(new Vector3(0f, 0f, angle), nudgeDuration)
            .SetEase(Ease.OutCubic)
            .SetId(nudgeTweenId)
            .OnComplete(() =>
            {
                visual.DOLocalRotate(Vector3.zero, returnDuration)
                    .SetEase(Ease.OutCubic)
                    .SetId(nudgeTweenId)
                    .OnComplete(() => nudgeInProgress = false);
            });
    }

    public void SetDraggStatus(bool value)
    {
        isDragging = value;

        DOTween.Kill(labelTweenId);
        Color targetColor = label.color;
        targetColor.a = value ? 0f : 1f;

        label.DOColor(targetColor, 0.2f).SetId(labelTweenId);

        canvas.overrideSorting = value;
        canvas.sortingOrder = value ? 100 : transform.GetSiblingIndex();

        lastMousePosition = Input.mousePosition;
        SetAlpha(1f);
    }

    public void Reset()
    {
        DOTween.Kill(rotateTweenId);
        visual.DOLocalRotate(Vector3.zero, resetToDefaultDuration)
            .SetEase(Ease.OutQuad)
            .SetId(rotateTweenId);
    }

    public void PointerEnter()
    {
        visual.DOScale(hoverScale, animDuration).SetEase(Ease.OutBack);
    }

    public void Drag()
    {
        visual.DOScale(hoverScale, animDuration).SetEase(Ease.OutBack);
    }

    public void PointerExit()
    {
        visual.DOScale(1f, animDuration).SetEase(Ease.OutBack);
    }

    public RectTransform GetVisual()
    {
        return visual;
    }
}
