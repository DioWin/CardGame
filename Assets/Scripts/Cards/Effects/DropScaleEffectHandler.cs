using UnityEngine;
using DG.Tweening;

public class DropScaleEffectHandler : MonoBehaviour, ICardEffect
{
    [Header("Scale On Drop Settings")]
    [SerializeField] private float targetScale = 0.8f;
    [SerializeField] private float duration = 0.2f;
    [SerializeField] private Ease ease = Ease.OutBack;

    private CardView view;
    private CardController controller;
    private Vector3 originalScale;

    private string tweenId => $"DropScale_{GetInstanceID()}";

    private void Awake()
    {
        view = GetComponent<CardView>();
        controller = GetComponent<CardController>();

        originalScale = Vector3.one;
        Init(view, controller);
    }

    public void Init(CardView view, CardController controller)
    {
        this.view = view;
        this.controller = controller;

        originalScale = view.GetVisual().localScale;

        controller.OnReleaseEvent += HandleDrop;
        controller.OnDragStatusChangedEvent += HandleDragStartEnd;
    }

    private void OnDestroy()
    {
        controller.OnDragStatusChangedEvent -= HandleDragStartEnd;
    }

    private void HandleDragStartEnd(bool isDragging)
    {
        if (isDragging)
        {
            // Reset to original when dragging starts
            DOTween.Kill(tweenId);
            view.GetVisual().localScale = originalScale;
        }
    }

    private void HandleDrop()
    {
        DOTween.Kill(tweenId);
        view.GetVisual().DOScale(targetScale, duration)
            .SetEase(ease)
            .SetId(tweenId);
    }
}
