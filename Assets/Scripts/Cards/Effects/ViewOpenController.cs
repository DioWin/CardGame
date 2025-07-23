using DG.Tweening;
using UnityEditor;
using UnityEngine;

public class ViewOpenController : MonoBehaviour
{
    [SerializeField] private GameObject view;
    [SerializeField] private GameObject scaleView;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float duration;
    [SerializeField] private Ease ease;

    public void Show()
    {
        view.SetActive(true);
        canvasGroup.alpha = 0;

        scaleView.transform.localScale = Vector3.zero;

        scaleView.transform.DOScale(1, duration);
        canvasGroup.DOFade(1, duration);
    }

    public void Hide()
    {
        scaleView.transform.DOScale(0, duration);

        canvasGroup.DOFade(0, duration).SetEase(ease).OnComplete(() =>
        {
            view.SetActive(false);
        });
    }
}
