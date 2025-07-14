using UnityEngine;

public interface IRuntimeSpell
{
    void Init(SpellBase config, GameObject caster, CardController cardController, Vector3 throwVelocity);
    void UpdateSpell();
    SpellState CurrentState { get; }

    void Show();
    void Hide();
}