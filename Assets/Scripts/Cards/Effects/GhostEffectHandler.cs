using UnityEngine;

public class GhostEffectHandler : MonoBehaviour
{
    [Header("Squeeze Fade Values")]

    [SerializeField] private float startGhostOffset = 10f;
    [SerializeField] private float endGhostOffset = 15f;

    [SerializeField] private float startGhoste = 1f;
    [SerializeField] private float endGhoste = 0f;

    private static readonly int FadeProp = Shader.PropertyToID("_Saturation");

    private CardView view;
    private CardController controller;
    private Material runtimeMaterial;

    private CardMaterialController initalizator;

    private void Awake()
    {
        view = GetComponent<CardView>();
        controller = GetComponent<CardController>();
        initalizator = GetComponent<CardMaterialController>();

        Init(view, controller);
    }

    public void Init(CardView view, CardController controller)
    {
        this.view = view;
        this.controller = controller;
        view.background.material = initalizator.GetRuntimeMaterial();
        runtimeMaterial = initalizator.GetRuntimeMaterial();
        runtimeMaterial.SetFloat(FadeProp, startGhoste);

        controller.OnDragDistanceChangedEvent += HandleDragDistanceChanged;
    }

    private void OnDestroy()
    {
        if (controller != null)
        {
            controller.OnDragDistanceChangedEvent -= HandleDragDistanceChanged;
        }
    }

    private void HandleDragDistanceChanged(float deltaY)
    {
        float t = Mathf.InverseLerp(endGhostOffset, startGhostOffset, deltaY);

        runtimeMaterial.SetFloat(FadeProp, t);
    }
}
