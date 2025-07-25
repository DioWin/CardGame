using UnityEngine;

public class GameplayManager : Singleton<GameplayManager>
{
    [Header("References")]
    [SerializeField] private CardHandController handController;
    [SerializeField] private DrawCardAnimator drawCardAnimator;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private CardModel[] startingDeckTemplates;
    [SerializeField] private DeckViewerController deckViewer;
    [SerializeField] private int startHandSize = 5;

    [Header("Timing")]
    [SerializeField] private float roundStartDelay = 2.5f;

    private DeckService deckService;
    private GameState currentState;

    private bool isStartingRound = false;
    private float roundStartTime;

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

    private void Update()
    {
        if (isStartingRound && Time.time >= roundStartTime)
        {
            isStartingRound = false;
            BeginRound();
        }
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
                PrepareRound();
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

    private void PrepareRound()
    {
        roundStartTime = Time.time + roundStartDelay;
        isStartingRound = true;
    }

    private void BeginRound()
    {
        EnemySpawner.Instance.StartWave();

        deckService.ResetRoundState();
        deckService.Shuffle();

        DrawCards(startHandSize);

        ChangeState(GameState.Combat);
    }

    public void DrawCards(int amount)
    {
        var cards = deckService.Draw(amount);
        if (cards.Count > 0)
            drawCardAnimator.StartAnimateDraw(cards, handController, cardPrefab);
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
