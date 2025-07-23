using UnityEngine;

public class FadeEffectHandler : MonoBehaviour, ICardEffect
{
    private BaseCardView view;
    private CardController controller;

    [Header("Fade Settings")]
    [SerializeField] private float fadeStartOffset = 10f;
    [SerializeField] private float fadeEndOffset = 15f;

    private void Awake()
    {
        view = GetComponent<GameCardView>();
        controller = GetComponent<CardController>();

        Init(view, controller);
    }

    public void Init(BaseCardView view, CardController controller)
    {
        this.view = view;
        this.controller = controller;

        controller.OnDragDistanceChangedEvent += HandleDragDistanceChanged;
    }

    private void OnDestroy()
    {
        if (controller != null)
            controller.OnDragDistanceChangedEvent -= HandleDragDistanceChanged;
    }

    private void HandleDragDistanceChanged(float deltaY)
    {
        float t = Mathf.InverseLerp(fadeEndOffset, fadeStartOffset, deltaY);
        view.SetAlpha(t);
    }
}

