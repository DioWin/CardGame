// CardHandController.cs
using System.Collections.Generic;
using UnityEngine;

public class CardHandController : MonoBehaviour
{
    public List<CardController> cards = new List<CardController>();
    public RectTransform visualHendler;
    public CurveParameters curveParameters;

    private void Awake()
    {
        UpdateList();
    }

    public void UpdateList()
    {
        cards.AddRange(GetComponentsInChildren<CardController>());
        for (int i = 0; i < cards.Count; i++)
            cards[i].Init(cards[i].model, visualHendler);

        ApplyCurveLayout();
    }

    private void Update()
    {
        cards.RemoveAll(c => c == null);

        UpdateCardSiblingOrder();
        ApplyCurveLayout();
    }

    public void UpdateCardSiblingOrder()
    {
        if (cards.Count == 0) return;

        CardController draggedCard = cards.Find(c => c.IsBeingDragged);
        if (draggedCard == null) return;

        float draggedX = draggedCard.GetVisualTransform().position.x;

        int newIndex = 0;
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i] == draggedCard) continue;

            if (draggedX > cards[i].GetVisualTransform().position.x)
                newIndex = Mathf.Max(newIndex, cards[i].transform.GetSiblingIndex() + 1);
        }

        draggedCard.SetSiblingIndex(newIndex);
    }

    private void ApplyCurveLayout()
    {
        if (cards.Count == 0 || curveParameters == null) return;

        int cardCount = transform.childCount;

        for (int i = 0; i < cardCount; i++)
        {
            var card = transform.GetChild(i).GetComponent<CardController>();

            if (card == null) continue;

            float t = cardCount == 1 ? 0.5f : (float)i / (cardCount - 1);
            float x = Mathf.Lerp(-curveParameters.curveWidth / 2f, curveParameters.curveWidth / 2f, t);
            float y = curveParameters.positionCurve.Evaluate(t) * curveParameters.curveHeight;

            Vector2 targetPos = new Vector2(x, y);
            card.transform.localPosition = targetPos;

            float angle = curveParameters.rotationCurve.Evaluate(t) * curveParameters.rotationInfluence;
            card.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
        }
    }
}
