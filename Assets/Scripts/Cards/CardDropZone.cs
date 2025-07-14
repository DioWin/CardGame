using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class CardDropZone : MonoBehaviour
{
    //[SerializeField] private Camera mainCamera;

    //public void OnDrop(PointerEventData eventData)
    //{
    //    if (eventData.pointerDrag == null) return;

    //    CardController card = eventData.pointerDrag.GetComponent<CardController>();
    //    var throwVelocity = card.GetThrowVelocity();

    //    if (card == null) return;

    //    Debug.Log(throwVelocity);

    //    InstantCast(card, throwVelocity);
    //}

    //private void InstantCast(CardController card, CardController cardController, Vector3 throwVelocity)
    //{
    //    Vector3 dropPosition = GetDropWorldPosition();

    //    SpellCaster caster = SpellCaster.Instance;
    //    caster.CastSpell(card.model.spell, cardController, throwVelocity);
    //    card.DestroyCard();
    //}

    //private Vector3 GetDropWorldPosition()
    //{
    //    Vector3 screenPoint = Input.mousePosition;
    //    screenPoint.z = Mathf.Abs(mainCamera.transform.position.z);

    //    Vector3 worldPoint = mainCamera.ScreenToWorldPoint(screenPoint);
    //    worldPoint.z = 0f;

    //    return worldPoint;
    //}
}
