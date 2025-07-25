using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DeckViewerSpellSection : MonoBehaviour
{
    public event Action<CardInstance> OnHoveredEnter;
    public event Action<CardInstance> OnHoveredExit;

    [SerializeField] private Transform container;
    [SerializeField] private TMP_Text counter;
    [SerializeField] private TMP_Text name;

    private SpellType type;
    private RectTransform visualParent;

    private readonly List<CardInstance> instances = new();
    private readonly Dictionary<Guid, CardController> cards = new();
    private int initialCount = 0;

    public void Init(SpellType type, RectTransform visualParent)
    {
        name.text = type.ToString();

        this.type = type;
        this.visualParent = visualParent;
        Clear();
    }

    public void AddCard(CardInstance instance, GameObject cardPrefab)
    {
        instances.Add(instance);
        initialCount++;

        var go = Instantiate(cardPrefab, container);
        var controller = go.GetComponent<CardController>();
        controller.Init(instance, visualParent);

        var cardView = go.GetComponent<DeckViewerCardView>();
        cardView.Init(instance, this.transform);
        cardView.OnHoveredEnter += HandleCardHoveredEnter;
        cardView.OnHoveredExit += HandleCardHoveredExit;

        cards[instance.UniqueId] = controller;

        UpdateCounter();
    }

    private void HandleCardHoveredEnter(CardInstance cardInstance)
    {
        OnHoveredEnter?.Invoke(cardInstance);
    }

    private void HandleCardHoveredExit(CardInstance cardInstance)
    {
        OnHoveredExit?.Invoke(cardInstance);
    }

    public void RemoveCard(CardInstance instance)
    {
        instances.RemoveAll(c => c.UniqueId == instance.UniqueId);

        if (cards.TryGetValue(instance.UniqueId, out var go))
        {
            go.DestroyCard();

            var cardView = go.GetComponent<DeckViewerCardView>();
            cardView.OnHoveredEnter -= HandleCardHoveredEnter;
            cardView.OnHoveredExit -= HandleCardHoveredExit;

            cards.Remove(instance.UniqueId);
        }

        UpdateCounter();
    }

    public void Clear()
    {
        instances.Clear();
        cards.Clear();
        initialCount = 0;

        foreach (Transform child in container)
            Destroy(child.gameObject);

        UpdateCounter();
    }

    public int GetRemaining()
    {
        return instances.Count(c => !c.IsInHand && !c.IsDestroyed);
    }

    public int GetTotal()
    {
        return initialCount;
    }

    public SpellType GetSpellType()
    {
        return type;
    }

    private void UpdateCounter()
    {
        counter.text = $"{GetRemaining()}/{initialCount}";
    }

    public List<CardInstance> GetCards()
    {
        return instances;
    }
}
