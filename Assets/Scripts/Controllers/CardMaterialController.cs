using UnityEngine;

public class CardMaterialController : MonoBehaviour
{
    [Header("Shader Settings")]
    [SerializeField] private Material baseMaterial;
    private Material runtimeMaterial;
    private bool isInitalized;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        if (isInitalized)
            return;

        runtimeMaterial = new Material(baseMaterial);

        isInitalized = true;
    }

    public Material GetRuntimeMaterial()
    {
        if (runtimeMaterial == null)
        {
            runtimeMaterial = new Material(baseMaterial);
            isInitalized = true;
        }

        return runtimeMaterial;
    }
}
