using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Card")]
public class CardModel : ScriptableObject
{
    public string cardName;
    public Sprite icon;
    public SpellType spellType;

    public SpellBase spell;
}
