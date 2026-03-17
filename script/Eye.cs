using Godot;
using System;

public partial class Eye : Node2D
{
    [Export] private Node2D _eyeball;
    [Export] private Sprite2D _iris;
    [Export] private Sprite2D _eyelid;
    [Export] private Sprite2D _color;
    [Export] private AnimationPlayer _animation;
    private double _timer = 3.0;
    private double blinkTimer = 6.0;

    public override void _Process(double delta)
    {
        _timer -= delta;
        if (_timer <= 0)
        {
            _timer = GD.RandRange(0.3,3);

            MoveEyeball(5);

            float randomScale = (float)GD.RandRange(0.5, 1.5);
            ScaleIris(randomScale, 0.5f);
        }

        blinkTimer -= delta;
        if (blinkTimer <= 0)
        {
            blinkTimer = GD.RandRange(0.2,8);
            _animation.Play("Blink");
        }
    }

    public void MoveEyeball(float range)
    {
        Vector2 randomPos = new((float)GD.RandRange(-range, range), (float)GD.RandRange(-range, range));
        _eyeball.Position = randomPos;
    }

    private Tween irisTween;
    public void ScaleIris(float targetScale, float duration = 0.2f)
    {
        if (_iris == null) return;
        if(duration <= float.Epsilon)
        {
            irisTween?.Kill();
            _iris.Scale = new(targetScale, targetScale);
        }
        else
        {
            irisTween = CreateTween();
            irisTween.TweenProperty(_iris, "scale", new Vector2(targetScale, targetScale), duration)
                .SetTrans(Tween.TransitionType.Quart)
                .SetEase(Tween.EaseType.Out);
        }
    }

    public void SetEyelidColor(Color color)
    {
        _eyelid.Modulate = color;
        // if (_eyelid.Material is ShaderMaterial mat)
        // {
        // 	mat.SetShaderParameter("target_color", color);
        // }
    }

    public void SetEyeColor(Color color)
    {
        _color.Modulate = color;
        // if (_color.Material is ShaderMaterial mat)
        // {
        // 	mat.SetShaderParameter("target_color", color);
        // }
    }
}
