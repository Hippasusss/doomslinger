using Godot;
using System;

public partial class Eye : Node2D
{
	[Export] private Node2D _eyeball;
	[Export] private Sprite2D _iris;
	[Export] private AnimationPlayer _animation;
	private double _timer = 3.0;
	private double blinkTimer = 6.0;

	public override void _Process(double delta)
	{
		_timer -= delta;
		if (_timer <= 0)
		{
			_timer = GD.RandRange(0.3,3);
			
			Vector2 randomPos = new(GD.RandRange(-5, 5), GD.RandRange(-5, 5));
			MoveEyeball(randomPos);

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

	public void MoveEyeball(Vector2 targetPosition)
	{
		_eyeball.Position = targetPosition;
	}

	public void ScaleIris(float targetScale, float duration = 0.2f)
	{
		if (_iris != null)
		{
			Tween tween = CreateTween();
			tween.TweenProperty(_iris, "scale", new Vector2(targetScale, targetScale), duration)
				 .SetTrans(Tween.TransitionType.Quart)
				 .SetEase(Tween.EaseType.Out);
		}
	}
}
