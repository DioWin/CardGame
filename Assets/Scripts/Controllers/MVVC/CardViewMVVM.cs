// CardViewMVVM.cs
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using System;

public class CardViewMVVM : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text label;
    [SerializeField] private RectTransform visual;
    [SerializeField] private Canvas canvas;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Drag Settings")]
    [SerializeField] private float dragSpeed = 20f;
    [SerializeField] private float followDefaultPosSpeed = 20f;
    [SerializeField] private float throwThreshold = 0.5f;
    [SerializeField] private float fadeStartOffset = 10f;
    [SerializeField] private float fadeEndOffset = 15f;

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

    public event Action OnBeginDragEvent;
    public event Action OnEndDragEvent;
    public event Action OnThrowConfirmedEvent;
    public event Action OnPointerEnterEvent;
    public event Action OnPointerExitEvent;
    public event Action<float> OnDragDistanceChangedEvent;

    public Func<float> FollowThresholdProvider;

    private Vector2 lastMousePosition;
    private float currentTilt;
    private float tiltVelocity;
    private bool isDragging;
    private bool nudgeInProgress;
    private bool isInitialized;
    private Transform modelRect;
    private Vector3 dragStartPosition;

    private string labelTweenId => $"LabelColor_{GetInstanceID()}";
    private string nudgeTweenId => $"Nudge_{GetInstanceID()}";
    private string rotateTweenId => $"Rotate_{GetInstanceID()}";

    private CardViewModel _viewModel;

    public void Bind(CardViewModel vm)
    {
        _viewModel = vm;

        label.text = _viewModel.Instance.Template.cardName;
        if (_viewModel.Instance.Template.icon != null)
            icon.sprite = _viewModel.Instance.Template.icon;
        else
            icon.gameObject.SetActive(false);

        _viewModel.OnDestroyed += HandleDestroyed;
        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized) return;

        if (isDragging)
        {
            float deltaY = visual.anchoredPosition.y - dragStartPosition.y;
            OnDragDistanceChangedEvent?.Invoke(deltaY);

            FollowCursor();
            HandleRotationToMouse();
        }
        else if (modelRect != null)
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

    private void HandleRotationToMouse()
    {
        Vector2 mouseDelta = (Vector2)Input.mousePosition - lastMousePosition;
        lastMousePosition = Input.mousePosition;

        float targetTilt = Mathf.Clamp(-mouseDelta.x * rotationMultiplier, -maxTiltAngle, maxTiltAngle);
        currentTilt = Mathf.SmoothDamp(currentTilt, targetTilt, ref tiltVelocity, tiltSmoothTime);
        visual.localRotation = Quaternion.Euler(0f, 0f, currentTilt);
    }

    private void LerpToRotation(float targetZ, float lerpSpeed)
    {
        float currentZ = visual.localEulerAngles.z;
        if (targetZ - currentZ > 180f) currentZ += 360f;
        else if (currentZ - targetZ > 180f) targetZ += 360f;

        float lerpedZ = Mathf.Lerp(currentZ, targetZ, Time.deltaTime * lerpSpeed);
        visual.localRotation = Quaternion.Euler(0f, 0f, lerpedZ);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        canvas.overrideSorting = true;
        canvas.sortingOrder = 100;
        canvasGroup.blocksRaycasts = false;

        lastMousePosition = Input.mousePosition;
        dragStartPosition = visual.anchoredPosition;

        OnBeginDragEvent?.Invoke();
    }

    public void OnDrag(PointerEventData eventData) { }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        canvas.sortingOrder = transform.GetSiblingIndex();
        canvasGroup.blocksRaycasts = true;

        float deltaY = visual.anchoredPosition.y - dragStartPosition.y;
        float t = Mathf.InverseLerp(fadeEndOffset, fadeStartOffset, deltaY);

        if (t < throwThreshold)
        {
            _viewModel.Play();
            OnThrowConfirmedEvent?.Invoke();
            _viewModel.Destroy();
        }

        OnEndDragEvent?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        visual.DOScale(hoverScale, animDuration).SetEase(Ease.OutBack);
        OnPointerEnterEvent?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        visual.DOScale(1f, animDuration).SetEase(Ease.OutBack);
        OnPointerExitEvent?.Invoke();
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

    private void HandleDestroyed()
    {
        Destroy(visual.gameObject);
        Destroy(gameObject);
    }

    public void SetModelRect(Transform rect)
    {
        modelRect = rect;
    }

    public void SetVisual(Transform rect)
    {
        visual.SetParent(rect);
    }

    public RectTransform GetVisual() => visual;
    public CardViewModel ViewModel => _viewModel;
    public bool IsBeingDragged() => isDragging;
}