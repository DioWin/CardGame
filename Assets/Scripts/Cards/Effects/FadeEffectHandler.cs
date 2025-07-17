using UnityEngine;

public class FadeEffectHandler : MonoBehaviour, ICardEffect
{
    private CardView view;
    private CardController controller;

    [Header("Fade Settings")]
    [SerializeField] private float fadeStartOffset = 10f;
    [SerializeField] private float fadeEndOffset = 15f;

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

        controller.OnDragDistanceChanged += HandleDragDistanceChanged;
    }

    private void OnDestroy()
    {
        if (controller != null)
            controller.OnDragDistanceChanged -= HandleDragDistanceChanged;
    }

    private void HandleDragDistanceChanged(float deltaY)
    {
        float t = Mathf.InverseLerp(fadeEndOffset, fadeStartOffset, deltaY);
        view.SetAlpha(t);
    }
}

