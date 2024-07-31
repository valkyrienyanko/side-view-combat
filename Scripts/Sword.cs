namespace SideViewCombat;

public partial class Sword : Sprite2D
{
    Area2D hitbox;

    public override void _Ready()
    {
        hitbox = GetNode<Area2D>("Hitbox");
        hitbox.BodyEntered += body =>
        {
            GD.Print("We hit something! Yeah!");
        };
    }

    public void Attack()
    {
        GTween tween = new GTween(this);

        // Sword swings up
        tween.Animate("rotation", -70f.ToRadians(), 0.6)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.Out);

        // Sword stops for a bit
        tween.Delay(0.1);

        // Sword swings down
        tween.Animate("rotation", 100f.ToRadians(), 0.4)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);

        tween.Callback(() =>
        {
            // Create the hitbox
        });
    }
}
