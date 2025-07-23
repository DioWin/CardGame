using System;
using System.Collections.Generic;

public class HandViewModel
{
    public event Action<CardInstance> OnCardAdded;
    public event Action<CardInstance> OnCardRemoved;

    private List<CardInstance> _cards = new();
    public IReadOnlyList<CardInstance> Cards => _cards;

    public void AddCard(CardInstance instance)
    {
        _cards.Add(instance);
        OnCardAdded?.Invoke(instance);
    }

    public void RemoveCard(CardInstance instance)
    {
        if (_cards.Remove(instance))
            OnCardRemoved?.Invoke(instance);
    }

    public void Clear()
    {
        _cards.Clear();
    }
}