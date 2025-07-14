using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardEffectBase : ScriptableObject
{
    public abstract void Execute(CardContext context);
}
