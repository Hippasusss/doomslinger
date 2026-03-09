








using Godot;
using System.Collections.Generic;

public partial class Feed : Sprite2D
{
	private const int numBlocks = 12;
	private const int blockSpacing = 52;

	[Export]
	private PackedScene feedBlockSN;

	private readonly List<Node2D> feedBlocks = [];

	public override void _Ready()
	{
		for(int i = 0; i < numBlocks; i++)
		{
			Node2D newfeedblock = feedBlockSN.Instantiate<Node2D>();
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

		foreach (Node2D block in feedBlocks)
		{
			tween.TweenProperty(block, "position:y", block.Position.Y + blockSpacing, duration)
				.SetTrans(Tween.TransitionType.Quart)
				.SetEase(Tween.EaseType.Out);
		}

		tween.Chain().TweenCallback(Callable.From(() =>
		{
			foreach (Node2D block in feedBlocks)
			{
				if (block.Position.Y > 100)
				{
					ResetBlock(block);
				}
			}
		}));
	}

	private void ResetBlock(Node2D block)
	{

		block.Position = new Vector2(block.Position.X, (feedBlocks.Count - 2) * -blockSpacing);
		ApplyRandomColor(block);
	}

	private static void ApplyRandomColor(Node2D block)
	{

		float r = (float)GD.RandRange(0.0, 1.0);
		float g = (float)GD.RandRange(0.0, 1.0);
		float b = (float)GD.RandRange(0.0, 1.0);

		block.Modulate = new Color(r, g, b, 1.0f);
	}


}
