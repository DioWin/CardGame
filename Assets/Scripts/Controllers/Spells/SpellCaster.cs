using UnityEngine;

public class SpellCaster : Singleton<SpellCaster>
{
    public void CastSpell(SpellBase spell, CardController cardController, Vector3 throwVelocity)
    {
        spell.Activate(this.gameObject, cardController, throwVelocity);
    }
}