using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(SortingGroup))]
public class RenderQueueFollower : MonoBehaviour
{
    [SerializeField] private int baseValue = 1;
    [SerializeField] private int fallowValue = 101;
    private SortingGroup rend;
    private bool currentStatus;

    private void Awake()
    {
        rend = GetComponent<SortingGroup>();
    }

    public void Detach()
    {
        Destroy(this);
    }

    public void SetFallowStatus(bool isEnable)
    {
        if (currentStatus == isEnable) 
            return;

        currentStatus = isEnable;

        if (isEnable) 
        {
            rend.sortingOrder = fallowValue;
        }
        else
        {
            rend.sortingOrder = baseValue;
        }
    }
}
