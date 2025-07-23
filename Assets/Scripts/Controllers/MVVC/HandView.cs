using System.Collections.Generic;
using UnityEngine;

public class HandView : MonoBehaviour
{
    [SerializeField] private Transform handTranform;
    [SerializeField] private Transform visualHandler;
    [SerializeField] private CardViewMVVM cardPrefab;
    [SerializeField] private CurveParameters curveParameters;

    private Dictionary<CardInstance, CardViewMVVM> _cardViews = new();
    private HandViewModel _viewModel;

    public void Bind(HandViewModel viewModel)
    {
        _viewModel = viewModel;
        _viewModel.OnCardAdded += HandleCardAdded;
        _viewModel.OnCardRemoved += HandleCardRemoved;
    }

    private void HandleCardAdded(CardInstance instance)
    {
        var viewModel = new CardViewModel(instance);
        var cardGO = Instantiate(cardPrefab, handTranform);
        cardGO.Bind(viewModel);
        cardGO.SetModelRect(cardGO.transform);
        cardGO.SetVisual(visualHandler);
        _cardViews[instance] = cardGO;

        ApplyCurveLayout();
    }

    private void Update()
    {
        UpdateCardSiblingOrder();
        ApplyCurveLayout();
    }

    private void HandleCardRemoved(CardInstance instance)
    {
        if (_cardViews.TryGetValue(instance, out var view))
        {
            Destroy(view.gameObject);
            _cardViews.Remove(instance);
        }
        ApplyCurveLayout();
    }

    private void ApplyCurveLayout()
    {
        var values = new List<CardViewMVVM>(_cardViews.Values);

        if (values.Count == 0 || curveParameters == null) return;

        int cardCount = values.Count;
        for (int i = 0; i < cardCount; i++)
        {
            var card = values[i];
            float t = cardCount == 1 ? 0.5f : (float)i / (cardCount - 1);
            float x = Mathf.Lerp(-curveParameters.curveWidth / 2f, curveParameters.curveWidth / 2f, t);
            float y = curveParameters.positionCurve.Evaluate(t) * curveParameters.curveHeight;

            Vector2 targetPos = new Vector2(x, y);
            card.transform.localPosition = targetPos;

            float angle = curveParameters.rotationCurve.Evaluate(t) * curveParameters.rotationInfluence;
            card.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    private void UpdateCardSiblingOrder()
    {
        var cards = _cardViews.Values;
        CardViewMVVM dragged = null;

        foreach (var card in cards)
        {
            if (card.IsBeingDragged())
            {
                dragged = card;
                break;
            }
        }

        if (dragged == null) return;

        float draggedX = dragged.GetVisual().position.x;
        int oldIndex = dragged.transform.GetSiblingIndex();
        int newIndex = 0;

        foreach (var card in cards)
        {
            if (card == dragged) continue;
            if (draggedX > card.GetVisual().position.x)
                newIndex = Mathf.Max(newIndex, card.transform.GetSiblingIndex() + 1);
        }

        if (newIndex != oldIndex)
            dragged.transform.SetSiblingIndex(newIndex);
    }

}