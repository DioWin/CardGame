using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DeckViewerView : MonoBehaviour
{
    [Header("Spell Type Sections")]
    [SerializeField] private List<DeckViewerSpellSection> sections;
    [SerializeField] private List<SpellType> spellTypes;

    [Header("Card Prefab and Visual Root")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private RectTransform cardVisualKeeper;

    [Header("Remaining Cards Counter")]
    [SerializeField] private TMP_Text amountOfCardRemering;
    [SerializeField] private TMP_Text probabilityInfoText;

    public event Action<int, int> OnUpdateLeftCardAmount;

    private Dictionary<SpellType, DeckViewerSpellSection> sectionByType;

    public void Init()
    {
        sectionByType = new();

        for (int i = 0; i < sections.Count; i++)
        {
            sections[i].Init(spellTypes[i], cardVisualKeeper);
            sectionByType[sections[i].GetSpellType()] = sections[i];

            sections[i].OnHoveredEnter += HandleCardHoveredEnter;
            sections[i].OnHoveredExit += HandleCardHoveredExit;
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < sections.Count; i++)
        {
            sections[i].OnHoveredEnter -= HandleCardHoveredEnter;
            sections[i].OnHoveredExit -= HandleCardHoveredExit;
        }
    }

    public void Render(List<CardInstance> deck, List<CardInstance> hand)
    {
        foreach (var section in sections)
            section.Clear();

        foreach (var instance in deck.Concat(hand))
        {
            var type = instance.Template.spellType;
            if (sectionByType.TryGetValue(type, out var section))
            {
                section.AddCard(instance, cardPrefab);
            }
        }

        UpdateTotalLeftCount();
    }

    public void AddCard(CardInstance instance)
    {
        var type = instance.Template.spellType;
        if (sectionByType.TryGetValue(type, out var section))
        {
            section.AddCard(instance, cardPrefab);
            UpdateTotalLeftCount();
        }
    }

    public void RemoveCard(CardInstance instance)
    {
        var type = instance.Template.spellType;
        if (sectionByType.TryGetValue(type, out var section))
        {
            section.RemoveCard(instance);
            UpdateTotalLeftCount();
        }
    }

    private void UpdateTotalLeftCount()
    {
        int currentLeft = sections.Sum(s => s.GetRemaining());
        int totalInitial = sections.Sum(s => s.GetTotal());

        amountOfCardRemering.text = $"{currentLeft}/{totalInitial}";
        OnUpdateLeftCardAmount?.Invoke(currentLeft, totalInitial);
    }

    private void HandleCardHoveredEnter(CardInstance cardInstance)
    {
        HandleCardHovered(cardInstance);
    }

    private void HandleCardHoveredExit(CardInstance cardInstance)
    {
        probabilityInfoText.text = "0/0";
    }

    public void HandleCardHovered(CardInstance hoveredInstance)
    {
        int totalRemaining = GetTotalRemainingCards();
        int matchingRemaining = GetRemainingMatchingCards(hoveredInstance);

        float chance = (totalRemaining > 0) ? (float)matchingRemaining / totalRemaining * 100f : 0f;

        string cardName = hoveredInstance.Template.cardName;
        probabilityInfoText.text = $"{matchingRemaining}/{totalRemaining} left\n{chance:F1}%";
    }

    private int GetTotalRemainingCards()
    {
        return sectionByType.Values.Sum(section => section.GetRemaining());
    }

    private int GetRemainingMatchingCards(CardInstance instance)
    {
        var template = instance.Template;

        return sectionByType.TryGetValue(template.spellType, out var section)
            ? section.GetCards().Count(c => c.Template == template && !c.IsInHand && !c.IsDestroyed)
            : 0;
    }
}
