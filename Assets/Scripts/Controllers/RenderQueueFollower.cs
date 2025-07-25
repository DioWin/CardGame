using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(SortingGroup))]
public class RenderQueueFollower : MonoBehaviour
{
    [SerializeField] private int baseValue = 1;
    [SerializeField] private int fallowValue = 101;
    private SortingGroup rend;
    private Canvas perentCanvas;
    private bool currentStatus;

    private int prevCanvasValue = -1;

    private void Start()
    {
        rend = GetComponent<SortingGroup>();
        perentCanvas = GetComponentInParent<Canvas>();
    }

    public void Detach()
    {
        Destroy(this);
        Destroy(rend);
    }

    private void Update()
    {
        if (perentCanvas == null)
        {
            Debug.Log(transform.name);
            return;
        }

        if (perentCanvas.sortingOrder != prevCanvasValue)
        {
            prevCanvasValue = perentCanvas.sortingOrder;

            rend.sortingOrder = prevCanvasValue;
        }
    }
}
