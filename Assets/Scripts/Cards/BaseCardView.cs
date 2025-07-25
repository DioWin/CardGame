using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public abstract class BaseCardView : MonoBehaviour
{
    [SerializeField] protected Image icon;
    [SerializeField] public Image background;
    [SerializeField] protected TMP_Text label;
    [SerializeField] protected RectTransform visual;
    [SerializeField] protected Canvas canvas;
    [SerializeField] protected CanvasGroup canvasGroup;

    [Header("Visual FX")]
    [SerializeField] protected float resetToDefaultDuration = 0.2f;
    [SerializeField] protected float dragSpeed = 20f;
    [SerializeField] protected float followDefaultPosSpeed = 20f;

    [Header("Rotation")]
    [SerializeField] protected float rotationMultiplier = 1.2f;
    [SerializeField] protected float maxTiltAngle = 10f;
    [SerializeField] protected float tiltSmoothTime = 0.08f;

    [Header("Nudge Settings")]
    [SerializeField] protected float nudgeAngle = 10f;
    [SerializeField] protected float nudgeDuration = 0.08f;
    [SerializeField] protected float returnDuration = 0.12f;

    protected Transform modelRect;
    protected bool isInitialized;
    protected bool isDragging;
    protected bool nudgeInProgress;
    protected Vector2 lastMousePosition;
    protected float currentTilt;
    protected float tiltVelocity;

    protected string labelTweenId => $"LabelColor_{GetInstanceID()}";
    protected string nudgeTweenId => $"Nudge_{GetInstanceID()}";
    protected string rotateTweenId => $"Rotate_{GetInstanceID()}";

    public virtual void SetData(CardModel model, Transform modelRect, Transform visualContainer)
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

        canvas.sortingOrder = transform.GetSiblingIndex();
    }

    public virtual void SetAlpha(float alpha)
    {
        canvasGroup.alpha = alpha;
        canvasGroup.blocksRaycasts = alpha > 0.1f;
    }

    public virtual void SetDraggStatus(bool value)
    {
        isDragging = value;

        if (isDragging)
            canvas.sortingOrder = 100;
        else
        {
            canvas.sortingOrder = visual.GetSiblingIndex() + 1;
        }

        DOTween.Kill(labelTweenId);
        Color targetColor = label.color;
        targetColor.a = value ? 0f : 1f;

        label.DOColor(targetColor, 0.2f).SetId(labelTweenId);

        lastMousePosition = Input.mousePosition;
        SetAlpha(1f);
    }

    public virtual void Reset()
    {
        DOTween.Kill(rotateTweenId);
        visual.DOLocalRotate(Vector3.zero, resetToDefaultDuration)
            .SetEase(Ease.OutQuad)
            .SetId(rotateTweenId);
    }

    public virtual void NudgeFromDirection(float direction)
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

    protected virtual void FollowCursor()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            visual.parent as RectTransform,
            Input.mousePosition,
            Camera.main,
            out Vector2 localPoint
        );

        visual.anchoredPosition = Vector2.Lerp(visual.anchoredPosition, localPoint, Time.deltaTime * dragSpeed);
    }

    protected virtual void LerpToRotation(float targetZ, float lerpSpeed)
    {
        float currentZ = visual.localEulerAngles.z;

        if (targetZ - currentZ > 180f) currentZ += 360f;
        else if (currentZ - targetZ > 180f) targetZ += 360f;

        float lerpedZ = Mathf.Lerp(currentZ, targetZ, Time.deltaTime * lerpSpeed);
        visual.localRotation = Quaternion.Euler(0f, 0f, lerpedZ);
    }

    protected virtual void HandleRotationToMouse()
    {
        Vector2 mouseDelta = (Vector2)Input.mousePosition - lastMousePosition;
        lastMousePosition = Input.mousePosition;

        float targetTilt = Mathf.Clamp(-mouseDelta.x * rotationMultiplier, -maxTiltAngle, maxTiltAngle);
        currentTilt = Mathf.SmoothDamp(currentTilt, targetTilt, ref tiltVelocity, tiltSmoothTime);
        visual.localRotation = Quaternion.Euler(0f, 0f, currentTilt);
    }

    public RectTransform GetVisual() => visual;

    public virtual void UpdateSublingIndexAndCanvasSorting(int sublingIndex)
    {
        visual.SetSiblingIndex(sublingIndex);

        if(isDragging)
            return;

        canvas.sortingOrder = sublingIndex;
    }

    public void OverrideCanvas(bool isEnable)
    {
        canvas.overrideSorting = isEnable;
    }
}
