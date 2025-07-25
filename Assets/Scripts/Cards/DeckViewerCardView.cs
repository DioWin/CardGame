using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeckViewerCardView : BaseCardView
{
    public event System.Action<CardInstance> OnHoveredEnter;
    public event System.Action<CardInstance> OnHoveredExit;

    private CardInstance instance;
    private CardController cardController;

    public void Init(CardInstance instance, Transform visualContainer)
    {
        this.instance = instance;
        SetData(instance.Template, transform, visualContainer);

        cardController = GetComponent<CardController>();

        cardController.OnPointerEnterEvent += OnPointerEnter;
        cardController.OnPointerExitEvent += OnPointerExit;
    }

    private void OnDestroy()
    {
        cardController.OnPointerEnterEvent += OnPointerEnter;
        cardController.OnPointerExitEvent += OnPointerExit;
    }

    public void OnPointerEnter()
    {
        if (!isInitialized || instance == null)
            return;

        OnHoveredEnter?.Invoke(instance);
    }

    public void OnPointerExit()
    {
        if (!isInitialized || instance == null)
            return;

        OnHoveredExit?.Invoke(instance);
    }

    private void Update()
    {
        if (!isInitialized)
            return;

        if (isDragging)
        {
            FollowCursor();
            HandleRotationToMouse();

            OverrideCanvas(true);
            UpdateSublingIndexAndCanvasSorting(200);
        }
        else
        {
            UpdateSublingIndexAndCanvasSorting(modelRect.GetSiblingIndex());

            OverrideCanvas(false);

            visual.position = Vector3.Lerp(
                visual.position,
                modelRect.position,
                Time.deltaTime * followDefaultPosSpeed
            );

            if (!nudgeInProgress)
                LerpToRotation(modelRect.eulerAngles.z, 10f);
        }
    }

    private void UpdateSublingIndexAndCanvasSorting(int value)
    {
        canvas.sortingOrder = value;
    }
}
