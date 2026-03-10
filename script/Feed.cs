








using Godot;
using System.Collections.Generic;

public partial class Feed : Sprite2D
{
	private const int numBlocks = 12;
	private const int blockSpacing = 52;

	[Export] private PackedScene feedBlockSN;
	[Export] private Light2D screenLight;

	private readonly List<Feedblock> feedBlocks = [];

	public override void _Ready()
	{
		for(int i = 0; i < numBlocks; i++)
		{
			Feedblock newfeedblock = feedBlockSN.Instantiate<Feedblock>();
			AddChild(newfeedblock);
			feedBlocks.Add(newfeedblock);
			ResetBlock(newfeedblock);
		}
	}
 
	private double timer = 5;
	public override void _Process(double delta) 
	{
		timer -= delta;
		if(timer <= 0)
		{
			AdvanceFeed();
			timer = GD.RandRange(1.0, 4.0);
		}
	}

	private void AdvanceFeed()
	{
		Tween tween = CreateTween().SetParallel(true);
		float duration = 0.4f;

		foreach (Feedblock block in feedBlocks)
		{
			tween.TweenProperty(block, "position:y", block.Position.Y + blockSpacing, duration)
				.SetTrans(Tween.TransitionType.Quart)
				.SetEase(Tween.EaseType.Out);

			if (Mathf.Abs(block.Position.Y + blockSpacing) < 1.0f)
			{
			    tween.TweenProperty(screenLight, "color", block.Modulate, duration)
			    	.SetTrans(Tween.TransitionType.Quart)
			    	.SetEase(Tween.EaseType.Out);
			}


		}

		tween.Chain().TweenCallback(Callable.From(() =>
		{
			foreach (Feedblock block in feedBlocks)
			{
				if (block.Position.Y > 100)
				{
					ResetBlock(block);
				}
			}
		}));
	}

	private void ResetBlock(Feedblock block)
	{

		block.Position = new Vector2(block.Position.X, (feedBlocks.Count - 2) * -blockSpacing);
        block.SetColour(GD.Randf(), GD.Randf(), GD.Randf(), 1.0f);
	}



}
