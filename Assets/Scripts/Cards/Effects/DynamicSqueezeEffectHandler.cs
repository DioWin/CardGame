using UnityEngine;

public class DynamicSqueezeEffectHandler : MonoBehaviour, ICardEffect
{
    [SerializeField] private float startSqueezeOffset = 10f;
    [SerializeField] private float endSqueezeOffset = 150f;

    [SerializeField] private float minFade = 0.05f;
    [SerializeField] private float maxFade = 0.3f;

    [SerializeField] private float maxCenterOffset = 0.25f;

    [Header("Scale Settings")]
    [SerializeField] private float minScale = 0.92f;
    [SerializeField] private float maxScale = 1f;

    [SerializeField] private float centerShiftPower = 0.4f; // Чим менше — тим швидше рух центра

    private static readonly int FadeProp = Shader.PropertyToID("_SqueezeFade");
    private static readonly int CenterProp = Shader.PropertyToID("_SqueezeCenter");

    private CardView view;
    private CardController controller;
    private Material runtimeMaterial;
    private CardMaterialController initalizator;

    private Vector2 dragStartPos;

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

        runtimeMaterial = initalizator.GetRuntimeMaterial();
        view.background.material = runtimeMaterial;

        runtimeMaterial.SetFloat(FadeProp, 0f);
        runtimeMaterial.SetVector(CenterProp, new Vector2(0.5f, 0.5f));

        controller.OnDragStatusChangedEvent += HandleDragStatusChanged;
        controller.OnDragDistanceChangedEvent += HandleDragDistanceChanged;
        controller.OnReleaseEvent += HandleRelease;
    }

    private void OnDestroy()
    {
        controller.OnDragStatusChangedEvent -= HandleDragStatusChanged;
        controller.OnDragDistanceChangedEvent -= HandleDragDistanceChanged;
        controller.OnReleaseEvent -= HandleRelease;
    }

    private void HandleDragStatusChanged(bool isDragging)
    {
        if (isDragging)
        {
            dragStartPos = Input.mousePosition;
        }
        else
        {
            runtimeMaterial.SetFloat(FadeProp, 0f);
            runtimeMaterial.SetVector(CenterProp, new Vector2(0.5f, 0.5f));
            ResetScale();
        }
    }

    private void HandleDragDistanceChanged(float deltaY)
    {
        float absDeltaY = Mathf.Abs(deltaY);
        float t = Mathf.InverseLerp(startSqueezeOffset, endSqueezeOffset, absDeltaY);

        // Fade
        float fade = Mathf.Lerp(minFade, maxFade, t);
        runtimeMaterial.SetFloat(FadeProp, fade);

        // Center (прискорене зміщення)
        Vector2 currentMousePos = Input.mousePosition;
        Vector2 delta = currentMousePos - dragStartPos;
        Vector2 rawOffset = new Vector2(delta.x / Screen.width, delta.y / Screen.height);

        float centerT = Mathf.Pow(t, centerShiftPower); // Прискорене зростання
        Vector2 offset = rawOffset.normalized * Mathf.Min(rawOffset.magnitude, 1f) * centerT * maxCenterOffset;

        Vector2 center = new Vector2(0.5f, 0.5f) + offset;
        runtimeMaterial.SetVector(CenterProp, center);

        // Scale
        float scale = Mathf.Lerp(maxScale, minScale, t);
        view.GetVisual().localScale = Vector3.one * scale;
    }

    private void HandleRelease()
    {
        runtimeMaterial.SetFloat(FadeProp, 0f);
        runtimeMaterial.SetVector(CenterProp, new Vector2(0.5f, 0.5f));
        ResetScale();
    }

    private void ResetScale()
    {
        view.GetVisual().localScale = Vector3.one;
    }
}
