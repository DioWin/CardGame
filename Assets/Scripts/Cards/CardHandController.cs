using System.Collections.Generic;
using UnityEngine;

public class CardHandController : MonoBehaviour
{
    public List<CardController> cards = new List<CardController>();
    public RectTransform visualHendler;
    public CurveParameters curveParameters;

    private int lastDraggedIndex = -1;

    private void Awake()
    {
        UpdateList();
    }

    public void UpdateList()
    {
        cards.Clear();
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

    public void UpdateCardSiblingOrder()
    {
        if (cards.Count == 0) return;

        CardController draggedCard = cards.Find(c => c.IsBeingDragged);
        if (draggedCard == null)
        {
            lastDraggedIndex = -1;
            lastDraggedX = float.NaN;
            return;
        }

        float draggedX = draggedCard.GetVisualTransform().position.x;
        int oldIndex = draggedCard.transform.GetSiblingIndex();
        int newIndex = 0;

        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i] == draggedCard) continue;

            if (draggedX > cards[i].GetVisualTransform().position.x)
                newIndex = Mathf.Max(newIndex, cards[i].transform.GetSiblingIndex() + 1);
        }

        if (newIndex != oldIndex)
        {
            int crossedIndex = newIndex > oldIndex ? oldIndex + 1 : oldIndex - 1;

            if (crossedIndex >= 0 && crossedIndex < transform.childCount)
            {
                var crossedCard = transform.GetChild(crossedIndex).GetComponent<CardController>();

                if (crossedCard != null && crossedCard != draggedCard)
                {
                    float crossedX = crossedCard.GetVisualTransform().position.x;

                    if (!float.IsNaN(lastDraggedX) &&
                        ((lastDraggedX < crossedX && draggedX >= crossedX) ||
                         (lastDraggedX > crossedX && draggedX <= crossedX)))
                    {
                        float direction = Mathf.Sign(draggedX - lastDraggedX);
                        crossedCard.NudgeFromDirection(-direction);
                    }
                }
            }

            draggedCard.SetSiblingIndex(newIndex);
            lastDraggedIndex = newIndex;
        }

        lastDraggedX = draggedX;
    }


    private float lastDraggedX = float.NaN;
}
