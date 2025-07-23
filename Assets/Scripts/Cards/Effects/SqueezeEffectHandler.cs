using UnityEngine;

[RequireComponent(typeof(CardView), typeof(CardController))]
public class SqueezeEffectHandler : MonoBehaviour, ICardEffect
{
    [Header("Squeeze Fade Values")]

    [SerializeField] private float startSqueezeOffset = 10f;
    [SerializeField] private float endSqueezeOffset = 15f;

    [SerializeField] private float startFade = 0.05f;
    [SerializeField] private float endFade = 0.2f;
    [SerializeField] private float defaultFade = 0f;

    private static readonly int FadeProp = Shader.PropertyToID("_SqueezeFade");
    private static readonly int CenterProp = Shader.PropertyToID("_SqueezeCenter");


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

        runtimeMaterial.SetFloat(FadeProp, defaultFade);
        runtimeMaterial.SetVector(CenterProp, new Vector2(0.5f, 0.5f));

        controller.OnDragStatusChangedEvent += HandleDragStatusChanged;
        controller.OnDragDistanceChangedEvent += HandleDragDistanceChanged;
        controller.OnReleaseEvent += HandleRelease;
    }

    private void OnDestroy()
    {
        if (controller != null)
        {
            controller.OnDragStatusChangedEvent -= HandleDragStatusChanged;
            controller.OnDragDistanceChangedEvent -= HandleDragDistanceChanged;
            controller.OnReleaseEvent -= HandleRelease;
        }
    }

    private void HandleDragStatusChanged(bool isDragging)
    {
        runtimeMaterial.SetFloat(FadeProp, isDragging ? startFade : defaultFade);
    }

    private void HandleDragDistanceChanged(float deltaY)
    {
        float t = Mathf.InverseLerp(startSqueezeOffset, endSqueezeOffset , deltaY);

        runtimeMaterial.SetFloat(FadeProp, t);
        //Vector2 mouseViewport = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        //runtimeMaterial.SetVector(CenterProp, new Vector2(mouseViewport.x, mouseViewport.y));
    }

    private void HandleRelease()
    {
        //runtimeMaterial.SetFloat(FadeProp, followFade);
    }
}
