using DG.Tweening;
using UnityEngine;

public class ScaleOnHoverEffectHandler : MonoBehaviour, ICardEffect
{
    private BaseCardView view;
    private CardController controller;

    [Header("Hover Settings")]
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float defaultHoverScale = 1f;
    [SerializeField] private float animDuration = 0.15f;

    private void Awake()
    {
        view = GetComponent<BaseCardView>();
        controller = GetComponent<CardController>();
        Init(view, controller);
    }

    public void Init(BaseCardView view, CardController controller)
    {
        this.view = view;
        this.controller = controller;

        HandlePointerExit();

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
        view.GetVisual().DOScale(defaultHoverScale, animDuration);
    }

    private void HandleDragStatusChanged(bool isDragging)
    {
        HandlePointerEnter();
    }
}
