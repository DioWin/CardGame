// CardController.cs
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public event Action OnReleaseEvent;
    public event Action OnEndDraggingEvent;
    public event Action OnFollowStartEvent;
    public event Action OnFollowStopEvent;
    public event Action OnThrowConfirmedEvent;
    public event Action<bool> OnDragStatusChangedEvent;
    public event Action<float> OnDragDistanceChangedEvent;
    public event Action OnPointerEnterEvent;
    public event Action OnPointerExitEvent;

    public Func<float> FollowThresholdProvider;

    public CardModel model;

    [Header("References")]
    [SerializeField] private CardView view;
    [SerializeField] private CanvasGroup canvas;

    [Header("Drag Settings")]
    [SerializeField] private float dragSpeed = 20f;
    [SerializeField] private float fadeStartOffset = 10f;
    [SerializeField] private float fadeEndOffset = 15f;
    [SerializeField] private float throwThreshold = 0.5f;

    public MouseVelocityTracker velocityTracker;

    private CanvasGroup canvasGroup;
    private Vector3 dragStartPosition;

    public bool IsBeingDragged { get; private set; }
    private bool isFollowing = false;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Init(CardModel data, RectTransform visualContainer)
    {
        model = data;
        view.SetData(model, this.transform, visualContainer);
        InstantCast(model, GetThrowVelocity());
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        IsBeingDragged = true;
        velocityTracker.ResetTracking();

        OnDragStatusChangedEvent?.Invoke(true);
        dragStartPosition = view.GetVisual().anchoredPosition;

        canvasGroup.blocksRaycasts = false;
        OnReleaseEvent?.Invoke();

        view.SetDraggStatus(true);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        IsBeingDragged = false;
        canvasGroup.blocksRaycasts = true;

        OnDragStatusChangedEvent?.Invoke(false);
        OnEndDraggingEvent?.Invoke();

        float deltaY = view.GetVisual().anchoredPosition.y - dragStartPosition.y;
        float t = Mathf.InverseLerp(fadeEndOffset, fadeStartOffset, deltaY);

        view.SetDraggStatus(false);

        if (t < throwThreshold)
        {
            OnThrowConfirmedEvent?.Invoke();
            DestroyCard();
        }
        else if (isFollowing)
        {
            OnFollowStopEvent?.Invoke();
            isFollowing = false;
        }
    }

    private void Update()
    {
        if (IsBeingDragged)
        {
            float deltaY = view.GetVisual().anchoredPosition.y - dragStartPosition.y;
            OnDragDistanceChangedEvent?.Invoke(deltaY);

            float threshold = FollowThresholdProvider?.Invoke() ?? float.MaxValue;
            bool shouldFollow = deltaY < threshold;

            if (shouldFollow != isFollowing)
            {
                if (shouldFollow)
                    OnReleaseEvent?.Invoke();
                else
                    OnFollowStopEvent?.Invoke();

                isFollowing = shouldFollow;
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
       
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnPointerEnterEvent?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnPointerExitEvent?.Invoke();
    }

    public Vector3 GetThrowVelocity()
    {
        return velocityTracker.AveragedVelocity;
    }

    public RectTransform GetVisualTransform()
    {
        return view.GetVisual();
    }

    public void SetSiblingIndex(int index)
    {
        transform.SetSiblingIndex(index);
        view.GetVisual().SetSiblingIndex(index);
    }

    public void DestroyCard()
    {
        Destroy(view.GetVisual().gameObject);
        Destroy(gameObject);
    }

    private void InstantCast(CardModel card, Vector3 throwVelocity)
    {
        SpellCaster.Instance.CastSpell(model.spell, this, throwVelocity);
    }

    public void NudgeFromDirection(float direction)
    {
        view.NudgeFromDirection(direction);
    }

    public int GetSiblingIndex()
    {
        return transform.GetSiblingIndex();
    }
}