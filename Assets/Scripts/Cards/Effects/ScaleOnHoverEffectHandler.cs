using DG.Tweening;
using UnityEngine;

public class ScaleOnHoverEffectHandler : MonoBehaviour, ICardEffect
{
    private CardView view;
    private CardController controller;

    [Header("Hover Settings")]
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float animDuration = 0.15f;

    private void Awake()
    {
        view = GetComponent<CardView>();
        controller = GetComponent<CardController>();
        Init(view, controller);
    }

    public void Init(CardView view, CardController controller)
    {
        this.view = view;
        this.controller = controller;

        controller.OnDragStatusChangedEvent += HandleDragStatusChanged;
        controller.OnPointerEnterEvent += HandlePointerEnter;
        controller.OnPointerExitEvent += HandlePointerExit;
    }

    private void OnDestroy()
    {
        if (controller != null)
        {
            controller.OnDragStatusChangedEvent -= HandleDragStatusChanged;
            controller.OnPointerEnterEvent -= HandlePointerEnter;
            controller.OnPointerExitEvent -= HandlePointerExit;
        }
    }

    private void HandlePointerEnter()
    {
        view.GetVisual().DOScale(hoverScale, animDuration);
    }

    private void HandlePointerExit()
    {
        view.GetVisual().DOScale(1, animDuration);
    }

    private void HandleDragStatusChanged(bool isDragging)
    {
        HandlePointerEnter();
    }
}
