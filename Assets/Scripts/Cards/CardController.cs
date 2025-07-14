using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class CardController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public event Action OnRelease;
    public event Action OnEndDragging;
    public event Action OnFollowStart;
    public event Action OnFollowStop;
    public event Action OnThrowConfirmed;

    public CardModel model;

    [Header("References")]
    [SerializeField] private CardView view;
    [SerializeField] private CardExplosionEffect explosionEffect;
    [SerializeField] private CanvasGroup canvas;
    [SerializeField] private float fadeDuration = 0.1f;

    public MouseVelocityTracker velocityTracker;

    [Header("Settings")]
    public float draggSpeed = 20f;

    [Header("Visual FX")]
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float animDuration = 0.15f;

    [Header("Visibility Fade Settings")]
    [SerializeField] private float fadeStartOffset = 10f;
    [SerializeField] private float fadeEndOffset = 15f;

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

        view.SetDraggStatus(true);
        dragStartPosition = view.GetVisual().anchoredPosition;

        canvasGroup.blocksRaycasts = false;

        OnRelease?.Invoke();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        IsBeingDragged = false;
        canvasGroup.blocksRaycasts = true;

        view.SetDraggStatus(false);
        view.Reset();

        float deltaY = view.GetVisual().anchoredPosition.y - dragStartPosition.y;
        float t = Mathf.InverseLerp(fadeEndOffset, fadeStartOffset, deltaY);

        OnEndDragging?.Invoke();

        if (t < 0.5f)
        {
            OnThrowConfirmed?.Invoke();
            DestroyCard();
        }else
        if (isFollowing)
        {
            OnFollowStop?.Invoke();
            isFollowing = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        view.GetVisual().DOScale(hoverScale, animDuration).SetEase(Ease.OutBack);
    }

    private void Update()
    {
        if (IsBeingDragged)
        {
            float deltaY = view.GetVisual().anchoredPosition.y - dragStartPosition.y;
            velocityTracker.Track();

            if (deltaY <= fadeStartOffset)
            {
                view.SetAlpha(1f);

                if (isFollowing)
                {
                    OnFollowStop?.Invoke();
                    isFollowing = false;
                }
            }
            else if (deltaY >= fadeEndOffset)
            {
                view.SetAlpha(0f);

                if (!isFollowing)
                {
                    OnRelease?.Invoke();
                    isFollowing = true;
                }
            }
            else
            {
                float t = Mathf.InverseLerp(fadeEndOffset, fadeStartOffset, deltaY);
                view.SetAlpha(t);

                bool shouldFollow = t < 0.5f;
                if (shouldFollow != isFollowing)
                {
                    if (shouldFollow)
                        OnRelease?.Invoke();
                    else
                        OnFollowStop?.Invoke();

                    isFollowing = shouldFollow;
                }
            }
        }
    }

    public Vector3 GetThrowVelocity()
    {
        return velocityTracker.AveragedVelocity;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        view.GetVisual().DOScale(hoverScale, animDuration).SetEase(Ease.OutBack);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        view.GetVisual().DOScale(1f, animDuration).SetEase(Ease.OutBack);
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
        //explosionEffect.Init(model.icon, view.GetVisual().transform.parent);
        //explosionEffect.Explode();

        Destroy(view.GetVisual().gameObject);
        Destroy(gameObject);
    }

    private void InstantCast(CardModel card, Vector3 throwVelocity)
    {
        SpellCaster caster = SpellCaster.Instance;
        caster.CastSpell(model.spell, this, throwVelocity);
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
