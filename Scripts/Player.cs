namespace SideViewCombat;

public partial class Player : Node
{
    Sword sword;

    State curState;

    public override void _Ready()
    {
        sword = GetNode<Sword>("Sword");

        curState = Idle();
        curState.Enter();
    }

    public override void _PhysicsProcess(double d)
    {
        float delta = (float)d;
        curState.Update(delta);
    }

    public void SwitchState(State newState)
    {
        curState.Exit();
        newState.Enter();
        curState = newState;

        GD.Print($"Switched to {newState}");
    }

    State Idle()
    {
        State state = new State(nameof(Idle));

        state.Enter = () =>
        {
            SwitchState(Attack());
        };

        return state;
    }

    State Attack()
    {
        State state = new State(nameof(Attack));

        state.Enter = () =>
        {
            sword.Attack(EndOfAttackAnimationCallback);
        };

        return state;
    }

    void EndOfAttackAnimationCallback()
    {
        SwitchState(Idle());
    }
}
