using Godot;
using System;
using System.Collections.Generic;

public partial class HumanSpawner : Node2D
{

	[Export] private HumanDataDisplay data;
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
		newHuman.Face.Texture = faces.GetRandom();
		newHuman.data = new HumanPersonalData(GetRandomName(),GetRandomDate(), GetRandomHeight(), GetRandomGender(), GetRandomNationality());
		newHuman.display = data;
	}

	private static string GetRandomName()
	{
		string[] names = { "John", "Jane", "Alice", "Bob", "Charlie", "Diana", "Edward", "Fiona", "George", "Hannah", "Ian", "Julia", "Kevin", "Laura", "Mike", "Nora", "Oscar", "Paula", "Quinn", "Ryan", "Sarah", "Tom", "Ursula", "Victor", "Wendy", "Xander", "Yara", "Zack" };
		return names.GetRandom();
	}

	private static int GetRandomHeight()
	{
		return GD.RandRange(140, 210);
	}

	private static string GetRandomGender()
	{
		string[] genders = { "M", "F", "NB" };
		return genders.GetRandom();
	}

	private static string GetRandomNationality()
	{
		string[] nationalities = { "Scottish", "English", "Welsh", "Irish", "French", "German", "Spanish", "Italian", "American", "Canadian", "Australian", "Japanese", "Chinese", "Indian", "Brazilian", "Mexican", "Russian", "Danish", "Swedish", "Norwegian" };
		return nationalities.GetRandom();
	}

	private static DateOnly GetRandomDate()
	{
		// 1. Define the hard-coded range
		DateOnly start = new(1920, 1, 1);
		DateOnly end = new(2010, 12, 31);

		// 2. Create a local random instance
		Random random = new();

		// 3. Calculate the total number of days in the range
		// DayNumber is the total days since Jan 1, 0001
		int startDays = start.DayNumber;
		int endDays = end.DayNumber;
		int range = endDays - startDays;

		// 4. Pick a random day within that range (Next is exclusive of the max)
		int randomOffset = random.Next(0, range + 1);

		// 5. Add that offset back to the start date
		return start.AddDays(randomOffset);
	}


}
