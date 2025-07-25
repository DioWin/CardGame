using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DrawCardAnimator : MonoBehaviour
{
    [SerializeField] private Transform deckVisualOrigin;
    [SerializeField] private float drawDelay = 0.15f;
    [SerializeField] private float moveDuration = 0.35f;


    public void StartAnimateDraw(List<CardInstance> instances, CardHandController handController, GameObject cardPrefab)
    {
        StartCoroutine(AnimateDraw(instances, handController, cardPrefab));
    }

    private IEnumerator AnimateDraw(List<CardInstance> instances, CardHandController handController, GameObject cardPrefab)
    {
        foreach (var instance in instances)
        {
            handController.SpawnCard(instance, cardPrefab);

            var card = handController.cards[handController.cards.Count - 1];
            var visual = card.GetVisualTransform();

            visual.position = deckVisualOrigin.position;
         
            yield return new WaitForSeconds(drawDelay);
        }
    }
}
