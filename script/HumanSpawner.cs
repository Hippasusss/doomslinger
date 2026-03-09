using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class HumanSpawner : Node2D
{
	[Export] private PackedScene human;
	[Export] private Node2D initPosition;
	[Export] private Texture2D[] faces;

	private const int numHumans = 4;
	private const int humanSpacing = 50;
	private List<Node2D> humans = new List<Node2D>();

	public override void _Ready()
	{
		for(int i = 0; i < numHumans; i++)
		{
			SpawnNewHuman();
		}
	}

	public override void _Process(double delta)
	{
	}

	private void SpawnNewHuman()
	{
		Node2D newHuman = human.Instantiate<Node2D>();
		AddChild(newHuman);
		humans.Add(newHuman);
		newHuman.Position = new Vector2(initPosition.Position.X + (humans.Count * humanSpacing), initPosition.Position.Y );
		var newHumanFace = newHuman.GetNode<Sprite2D>("Face");
		newHumanFace.Texture = faces.GetRandom();

		//TODO: random colours

	}
}
