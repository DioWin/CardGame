using UnityEngine;

public class SqueezeEffectHandler : MonoBehaviour, ICardEffect
{
    private CardView view;
    private CardController controller;

    [Header("Squeeze Settings")]
    [SerializeField] private float idleSqueeze = 0.05f;
    [SerializeField] private float dragSqueeze = 0.2f;

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

        controller.OnDragStatusChanged += HandleDragStatusChanged;
        controller.OnEndDraggingEvent += ResetSqueeze;
    }

    private void OnDestroy()
    {
        if (controller != null)
        {
            controller.OnDragStatusChanged -= HandleDragStatusChanged;
            controller.OnEndDraggingEvent -= ResetSqueeze;
        }
    }

    private void HandleDragStatusChanged(bool isDragging)
    {
        SetSqueeze(isDragging ? dragSqueeze : idleSqueeze);
    }

    private void ResetSqueeze()
    {
        SetSqueeze(0f);
    }

    private void SetSqueeze(float amount)
    {
        var visual = view.GetVisual();
        visual.localScale = new Vector3(1f, 1f - amount, 1f);
    }
}
