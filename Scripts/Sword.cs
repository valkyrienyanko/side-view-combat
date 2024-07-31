namespace SideViewCombat;

public partial class Sword : Sprite2D
{
    public void Attack(Action endOfAnimationCallback)
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
            // Create the Area2D hitbox
            Area2D area = new();
            CollisionShape2D collisionShape = new();

            RectangleShape2D shape = new();
            shape.Size = Vector2.One * 100;
            collisionShape.Shape = shape;

            area.AddChild(collisionShape);
            AddChild(area);

            area.AreaEntered += area =>
            {
                GD.Print("Woo hoo");
            };

            GTween tween = new GTween(this);
            tween.Delay(0.01);
            tween.Callback(() => area.QueueFree());

            endOfAnimationCallback();
        });
    }
}
