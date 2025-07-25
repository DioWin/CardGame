using UnityEngine;
using UnityEngine.UI;

public class AutoSpacingAdjuster : MonoBehaviour
{
    public HorizontalLayoutGroup layoutGroup;
    public RectTransform content;
    public float minSpacing = 0f;
    public float maxSpacing = 20f;

    void Update()
    {
        float totalWidth = 0f;
        foreach (RectTransform child in content)
        {
            if (child.gameObject.activeSelf)
                totalWidth += child.sizeDelta.x;
        }

        float availableWidth = content.rect.width;
        int count = content.childCount;

        float spacing = (availableWidth - totalWidth) / (count - 1);
        layoutGroup.spacing = Mathf.Clamp(spacing, minSpacing, maxSpacing);
    }
}
