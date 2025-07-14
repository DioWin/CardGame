using UnityEngine;

public abstract class SpellBase : ScriptableObject
{
    public string spellName;
    public Sprite icon;
    public SpellType spellType;
    public CastType castType;

    public GameObject runtimePrefab;

    public abstract void Activate(GameObject caster, CardController cardController, Vector3 throwVelocity);
    public abstract void Show();
    public abstract void Hide();
}