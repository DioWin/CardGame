using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIController : MonoBehaviour
{
    [SerializeField] private Button exitButton;
    [SerializeField] private Button drawCardButton;
    [SerializeField] private TextMeshProUGUI timer;
    [SerializeField] private float duration;
    private float startTime;

    private bool timeFinised;

    private void Start()
    {
        startTime = Time.time;
    }

    private void Update()
    {
        if (timeFinised)
            return;

        var elapsedTime = Time.time - startTime;
        var timeLeft = duration - elapsedTime;

        if (timeLeft < 0)
        {
            timer.text = "";
            timeFinised = true;
        }else
        {
            timer.text = $"{timeLeft.ToString("F0")}";
        }
    }

    private void Awake()
    {
        exitButton.onClick.AddListener(ExitFromGame);
        drawCardButton.onClick.AddListener(DrawOneCard);
    }

    private void ExitFromGame()
    {
        Application.Quit();
    }

    private void DrawOneCard()
    {
        GameplayManager.Instance.DrawCards(1);
    }
}
