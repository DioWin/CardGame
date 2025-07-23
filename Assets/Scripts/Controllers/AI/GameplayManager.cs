using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CardHandController handController;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private CardModel[] startingDeckTemplates;
    [SerializeField] private DeckViewerController deckViewer;
    [SerializeField] private int startHandSize = 5;

    private DeckService deckService;
    private GameState currentState;

    private void Start()
    {
        deckService = new DeckService();
        CreateInitialDeck();

        deckViewer.Init(deckService);

        ChangeState(GameState.Preparing);
    }

    private void CreateInitialDeck()
    {
        foreach (var model in startingDeckTemplates)
            deckService.AddCard(model);

        deckService.Shuffle();
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;

        switch (newState)
        {
            case GameState.Shop:
                // TODO: open shop UI
                break;

            case GameState.Preparing:
                StartNewRound();
                break;

            case GameState.Combat:
                // TODO: Spawn enemies
                break;

            case GameState.EndWave:
                // TODO: Save progress, rewards
                break;

            case GameState.Defeat:
                // TODO: Show defeat screen, offer to save deck
                break;
        }
    }

    private void StartNewRound()
    {
        EnemySpawner.Instance.StartWave();

        deckService.ResetRoundState();
        deckService.Shuffle();

        var drawnCards = deckService.Draw(startHandSize);

        foreach (var card in drawnCards)
            handController.SpawnCard(card, cardPrefab);

        ChangeState(GameState.Combat);
    }

    public void OnEnemyWaveCleared()
    {
        ChangeState(GameState.EndWave);
    }

    public void OnPlayerDefeated()
    {
        ChangeState(GameState.Defeat);
    }
}