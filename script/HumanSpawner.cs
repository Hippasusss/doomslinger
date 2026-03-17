using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class HumanSpawner : Node2D
{

	[Export] private HumanDataDisplay displayData;
	[Export] private CameraController camera;
	[Export] private PackedScene human;
	[Export] private Node2D initPosition;
	[Export] private int numHumans = 1;
	[Export] private FaceGenerator faceGenerator;
	private const int humanSpacing = 50;
	private readonly List<Human> humans = [];

	public override async void _Ready()
	{
		for(int i = 0; i < numHumans; i++)
		{
			await SpawnNewHuman();
		}
	}

	private async Task SpawnNewHuman()
	{
		Human newHuman = human.Instantiate<Human>();
		AddChild(newHuman);
		humans.Add(newHuman);
		newHuman.Position = new Vector2(initPosition.Position.X + (humans.Count * humanSpacing), initPosition.Position.Y );
		newHuman.Data = new HumanPersonalData(GetRandomName(),GetRandomDate(), GetRandomHeight(), GetRandomGender(), GetRandomNationality(), GetRandomEyeColour());
		
		// need to wait for it to draw the aggregate sprite
		var retVal = await faceGenerator.GenerateAsync();
		newHuman.Face.Texture = retVal.Item1;
		newHuman.Colors = retVal.Item2;
		newHuman.HumanSelected += camera.MovePositionToNode;
		newHuman.HumanSelected += displayData.DisplayNewHuman;
	}

	private static string GetRandomName()
	{
		string[] names = [ "John", "Jane", "Alice", "Bob", "Charlie", "Diana", "Edward", "Fiona", "George", "Hannah", "Ian", "Julia", "Kevin", "Laura", "Mike", "Nora", "Oscar", "Paula", "Quinn", "Ryan", "Sarah", "Tom", "Ursula", "Victor", "Wendy", "Xander", "Yara", "Zack" ];
		return names.GetRandom();
	}

	private static int GetRandomHeight()
	{
		return GD.RandRange(140, 210);
	}

	private static string GetRandomGender()
	{
		string[] genders = [ "M", "F", "NB" ];
		return genders.GetRandom();
	}

	private static string GetRandomNationality()
	{
		string[] nationalities = [ "Scottish", "English", "Welsh", "Irish", "French", "German", "Spanish", "Italian", "American", "Canadian", "Australian", "Japanese", "Chinese", "Indian", "Brazilian", "Mexican", "Russian", "Danish", "Swedish", "Norwegian" ];
		return nationalities.GetRandom();
	}

	private static DateOnly GetRandomDate()
	{
		DateOnly start = new(1920, 1, 1);
		DateOnly end = new(2010, 12, 31);

		Random random = new();

		int startDays = start.DayNumber;
		int endDays = end.DayNumber;
		int range = endDays - startDays;

		int randomOffset = random.Next(0, range + 1);

		return start.AddDays(randomOffset);
	}

	private static HumanPersonalData.EyeColour GetRandomEyeColour()
	{
		return (HumanPersonalData.EyeColour)GD.RandRange(0,2);
	}
}
