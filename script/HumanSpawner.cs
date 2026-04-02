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
    private static readonly string[] nationalities = [ "Scottish", "English", "Welsh", "Irish", "French", "German", "Spanish", "Italian", "American", "Canadian", "Australian", "Japanese", "Chinese", "Indian", "Brazilian", "Mexican", "Russian", "Danish", "Swedish", "Norwegian" ];
    private static readonly string[] femaleNames = [ "Jane", "Alice", "Diana", "Fiona", "Hannah", "Julia", "Laura", "Nora", "Paula", "Sarah", "Ursula", "Wendy", "Yara" ];
    private static readonly string[] maleNames = [ "John", "Bob", "Charlie", "Edward", "George", "Ian", "Kevin", "Mike", "Oscar", "Quinn", "Ryan", "Tom", "Victor", "Xander", "Zack" ];
    private static readonly string[] surnames = [ "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez", "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson" ];
    private static readonly string[] genders = [ "M", "F" ];

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
        string gender = GetRandomGender();
        newHuman.Data = new HumanPersonalData(GetRandomName(gender),GetRandomDate(), GetRandomHeight(gender), gender, GetRandomNationality() , 5);

        // need to wait for it to draw the aggregate sprite
        var (texture, colours) = await faceGenerator.GenerateAsync(newHuman.Data);
        newHuman.Face.Texture = texture;
        newHuman.Colors = colours;
        newHuman.HumanSelected += camera.MovePositionToNode;
        newHuman.HumanSelected += displayData.DisplayNewHuman;
        newHuman.Phone.Modulate = newHuman.Colors[5];
    }

    private static string GetRandomName(string gender)
    {
        string firstName;
        if (gender == "M")
        {
            firstName = maleNames.GetRandom();
        }
        else if (gender == "F")
        {
            firstName = femaleNames.GetRandom();
        }
        else
        {
            firstName = maleNames.GetRandom();
        }
        string lastName = surnames.GetRandom();
        return $"{firstName} {lastName}";
    }

    private static int GetRandomHeight(string gender)
    {
        if (gender == "M")
        {
            int baseHeight = GD.RandRange(140, 210);
            int skewedHeight = baseHeight + (int)((210 - baseHeight) * 0.3);
            return Mathf.Clamp(skewedHeight, 140, 210);
        }
        else if (gender == "F")
        {
            int baseHeight = GD.RandRange(140, 180);
            int skewedHeight = baseHeight - (int)((baseHeight - 140) * 0.3);
            return Mathf.Clamp(skewedHeight, 140, 180);
        }
        else
        {
            return GD.RandRange(140, 210);
        }
    }

    private static string GetRandomGender()
    {
        return genders.GetRandom();
    }

    private static string GetRandomNationality()
    {
        return nationalities.GetRandom();
    }

    private static DateOnly GetRandomDate()
    {
        DateOnly start = new(1940, 1, 1);
        DateOnly end = new(2010, 12, 31);

        Random random = new();

        int startDays = start.DayNumber;
        int endDays = end.DayNumber;
        int range = endDays - startDays;

        double randomOffset1 = random.NextDouble() * range;
        double randomOffset2 = random.NextDouble() * range;
        double averageOffset = (randomOffset1 + randomOffset2) / 2.0;

        return start.AddDays((int)Math.Round(averageOffset));
    }
}
