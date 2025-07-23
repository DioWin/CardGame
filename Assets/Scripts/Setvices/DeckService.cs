using System;
using System.Collections.Generic;
using System.Linq;

public class DeckService
{
    public event Action<CardInstance> OnCardAdded;
    public event Action<CardInstance> OnCardRemoved;
    public event Action OnDeckReset;

    public List<CardInstance> Deck { get; private set; } = new();
    public List<CardInstance> Hand { get; private set; } = new();

    public DeckService() { }

    public void AddCard(CardModel model)
    {
        var instance = new CardInstance(model);
        Deck.Add(instance);
        OnCardAdded?.Invoke(instance);
    }

    public void RemoveById(Guid id)
    {
        var removed = Deck.FirstOrDefault(c => c.UniqueId == id);
        if (removed != null)
        {
            Deck.Remove(removed);
            Hand.RemoveAll(c => c.UniqueId == id);
            OnCardRemoved?.Invoke(removed);
        }
    }

    public void Shuffle()
    {
        Deck = Deck.OrderBy(_ => UnityEngine.Random.value).ToList();
    }

    public List<CardInstance> Draw(int count)
    {
        var drawn = new List<CardInstance>();

        for (int i = 0; i < count && Deck.Count > 0; i++)
        {
            var card = Deck[0];
            Deck.RemoveAt(0);
            OnCardRemoved?.Invoke(card);

            card.IsInHand = true;
            card.WasPlayedThisRound = false;

            Hand.Add(card);
            drawn.Add(card);
        }

        return drawn;
    }

    public void ResetRoundState()
    {
        foreach (var c in Deck)
        {
            c.WasPlayedThisRound = false;
            c.IsInHand = false;
        }

        foreach (var c in Hand)
        {
            c.WasPlayedThisRound = false;
            c.IsInHand = false;
        }

        Hand.Clear();

        OnDeckReset?.Invoke();
    }

    public void EraseCardInHand(Guid id)
    {
        var card = Hand.FirstOrDefault(c => c.UniqueId == id);
        if (card != null)
        {
            card.IsDestroyed = true;
            Hand.Remove(card);

            OnCardRemoved?.Invoke(card);
        }

        //var deckCard = Deck.FirstOrDefault(c => c.UniqueId == id);
        //if (deckCard != null)
        //{
        //    Deck.Remove(deckCard);
        //    OnCardRemoved?.Invoke(deckCard);
        //}
    }

    public CardInstance PeekTop()
    {
        return Deck.FirstOrDefault();
    }

    public int RemainingDeckCount => Deck.Count;
}
