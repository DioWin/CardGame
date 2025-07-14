using UnityEngine;

[CreateAssetMenu(menuName = "Spells/FireballSpell")]
public class FireballSpell : SpellBase
{
    [Header("Fireball Parameters")]
    public float damage = 80f;
    public float speed = 15f;
    public float arcHeight = 2f;
    public float radius = 2f;
    public float falldownForce = 200f;

    public override void Activate(GameObject caster, CardController cardController, Vector3 throwVelocity)
    {
        GameObject go = Instantiate(runtimePrefab, caster.transform.position, Quaternion.identity);
        var runtime = go.GetComponent<IRuntimeSpell>();
        runtime.Init(this, caster, cardController, throwVelocity);
    }

    public override void Hide()
    {
    }

    public override void Show()
    {
    }
}
