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

    public CardInstance Instance { get; private set; }
    public CardModel Template => Instance?.Template;

    [Header("References")]
    [SerializeField] private BaseCardView view;
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
    private bool inHand = false;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Init(CardInstance instance, RectTransform visualContainer, bool inHand = false)
    {
        Instance = instance;

        view.SetData(instance.Template, this.transform, visualContainer);

        this.inHand = inHand;

        if (inHand)
            RenderSpell(instance.Template, GetThrowVelocity());
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

        view.SetDraggStatus(false);

        if (!inHand)
           return;

        float deltaY = view.GetVisual().anchoredPosition.y - dragStartPosition.y;
        float t = Mathf.InverseLerp(fadeEndOffset, fadeStartOffset, deltaY);


        if (t < throwThreshold)
        {
            Instance.IsDestroyed = true;
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

    public void OnDrag(PointerEventData eventData) { }

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

    private void RenderSpell(CardModel card, Vector3 throwVelocity)
    {
        SpellCaster.Instance.CastSpell(card.spell, this, throwVelocity);
        Instance.WasPlayedThisRound = true;
    }

    public void NudgeFromDirection(float direction)
    {
        view.NudgeFromDirection(direction);
    }

    public int GetSiblingIndex()
    {
        return transform.GetSiblingIndex();
    }

    private void OnDestroy()
    {
        Debug.Log("Destroy");
    }
}
