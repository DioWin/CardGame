using UnityEngine;

[CreateAssetMenu(menuName = "Spells/BombSpell")]
public class BombSpell : SpellBase
{
    [Header("Bomb Parameters")]
    public float damage = 100f;
    public float radius = 3f;
    public float delayBeforeExplosive = 3f;
    public float falldownForce = 200f;

    public override void Activate(GameObject caster, CardController cardController, Vector3 throwVelocity)
    {
        GameObject go = Instantiate(runtimePrefab, caster.transform.position, Quaternion.identity);
        var runtime = go.GetComponent<IRuntimeSpell>();
        runtime.Init(this, caster, cardController, throwVelocity);
    }

    public override void Show()
    {

    }

    public override void Hide()
    {

    }
}