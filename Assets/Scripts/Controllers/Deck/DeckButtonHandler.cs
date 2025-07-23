using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckButtonHandler : MonoBehaviour
{
    [SerializeField] private DeckViewerController viewerController;
    [SerializeField] private TextMeshProUGUI counter;

    [SerializeField] private Button button;

    private void Awake()
    {
        button.onClick.AddListener(OpenView);

        viewerController.view.OnUpdateLeftCardAmount += UpdateCounter;
    }

    private void OnDestroy()
    {
        viewerController.view.OnUpdateLeftCardAmount += UpdateCounter;
    }

    private void UpdateCounter(int cardLeft, int totalCard)
    {
        counter.text = cardLeft.ToString();
    }

    private void OpenView()
    {
        viewerController.Show();
    }
}
