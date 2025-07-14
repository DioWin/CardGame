using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIController : MonoBehaviour
{
    [SerializeField] private Button exitButton;
    [SerializeField] private Button refreshButton;

    public List<CardController> cards = new List<CardController>();
    public CardHandController cardHandController;

    private void Awake()
    {
        exitButton.onClick.AddListener(ExitFromGame);
        refreshButton.onClick.AddListener(RefreshCards);
    }

    private void RefreshCards()
    {
        foreach (CardController card in cards)
        {
            var newPrefab = Instantiate(card.gameObject, cardHandController.transform);
        }

        cardHandController.UpdateList();
    }

    private void ExitFromGame()
    {
        Application.Quit();
    }
}
