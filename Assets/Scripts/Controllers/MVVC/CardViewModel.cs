using System;

public class CardViewModel
{
    public CardInstance Instance { get; private set; }

    public event Action OnDestroyed;
    public event Action OnPlayed;

    public CardViewModel(CardInstance instance)
    {
        Instance = instance;
    }

    public void Play()
    {
        Instance.WasPlayedThisRound = true;
        OnPlayed?.Invoke();
    }

    public void Destroy()
    {
        Instance.IsDestroyed = true;
        OnDestroyed?.Invoke();
    }
}
