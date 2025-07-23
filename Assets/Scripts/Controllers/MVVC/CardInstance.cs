using System;

public class CardInstance
{
    public CardModel Template { get; private set; }

    public Guid UniqueId { get; private set; }

    public bool WasPlayedThisRound { get; set; }
    public bool IsInHand { get; set; }
    public bool IsDestroyed { get; set; }

    public CardInstance(CardModel template)
    {
        Template = template;
        UniqueId = Guid.NewGuid();
    }
}
