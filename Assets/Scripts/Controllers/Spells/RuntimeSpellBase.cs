using UnityEngine;

public abstract class RuntimeSpellBase : MonoBehaviour, IRuntimeSpell
{
    protected SpellBase config;
    protected GameObject caster;
    protected CardController cardController;
    protected Vector3 throwVelocity;
    public SpellState CurrentState { get; protected set; }
    [SerializeField] private SpellState currentStateDebug;

    public virtual void Init(SpellBase config, GameObject caster, CardController cardController, Vector3 throwVelocity)
    {
        this.config = config;
        this.caster = caster;
        this.cardController = cardController;
        this.throwVelocity = throwVelocity;
        CurrentState = SpellState.Preparing;
    }

    public virtual void UpdateSpell()
    {
        currentStateDebug = CurrentState;

        switch (CurrentState)
        {
            case SpellState.Preparing:
                OnPrepare();
                break;
            case SpellState.Active:
                OnActive();
                break;
            case SpellState.Finished:
                OnFinished();
                break;
        }
    }

    protected abstract void OnPrepare();
    protected abstract void OnActive();
    protected abstract void OnFinished();

    public virtual void Show()
    {
    }

    public virtual void Hide()
    {
    }
}