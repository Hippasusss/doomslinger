using Godot;
using System.Collections.Generic;

public partial class HumanSpawner : Node2D
{
	[Export] private PackedScene human;
	[Export] private Node2D initPosition;
	[Export] private Texture2D[] faces;
	[Export] private int numHumans = 1;
	private const int humanSpacing = 40;
	private readonly List<Human> humans = [];

	public override void _Ready()
	{
		for(int i = 0; i < numHumans; i++)
		{
			SpawnNewHuman();
		}
	}

	private void SpawnNewHuman()
	{
		Human newHuman = human.Instantiate<Human>();
		AddChild(newHuman);
		humans.Add(newHuman);
		newHuman.Position = new Vector2(initPosition.Position.X + (humans.Count * humanSpacing), initPosition.Position.Y );
		var newHumanFace = newHuman.GetNode<Sprite2D>("Face");
		newHumanFace.Texture = faces.GetRandom();
	}
}
