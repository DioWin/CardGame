using UnityEngine;
using UnityEngine.UI;

public class DeckViewerController : MonoBehaviour
{
    [SerializeField] public DeckViewerView view;
    [SerializeField] private ViewOpenController openViewController;
    [SerializeField] private Button exitButton;

    private DeckService deckService;

    private void Awake()
    {
        exitButton.onClick.AddListener(Hide);
    }

    public void Init(DeckService service)
    {
        deckService = service;

        view.Init();

        deckService.OnCardAdded += view.AddCard;
        deckService.OnCardRemoved += view.RemoveCard;
        deckService.OnDeckReset += Refresh;

        //Refresh();
    }

    public void Show()
    {
        openViewController.Show();
    }

    public void Hide()
    {
        openViewController.Hide();
    } 

    public void Refresh()
    {
        view.Render(deckService.Deck, deckService.Hand);
    }
}
