using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace DoomSlinger;

public partial class HumanSpawner : Node2D
{
    [Export] private HumanManager humanManager;
    [Export] private PackedScene human;
    [Export] private Node2D initPosition;
    [Export] private int numHumans = 1;
    [Export] private FaceGenerator faceGenerator;
    private const int humanSpacing = 50;
    [Export] private GameData gameData;

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
        humanManager.AddHuman(newHuman);
        newHuman.Position = new Vector2(initPosition.Position.X + (humanManager.HumanCount * humanSpacing), initPosition.Position.Y );
        HumanPersonalData.Gender gender = GetRandomGender();
        HumanPersonalData.Orientation orientation = GetRandomOrientation();
        newHuman.Data = new HumanPersonalData(GetRandomName(gender), GetRandomDate(), GetRandomHeight(gender), gender, orientation, GetRandomNationality(), GetRandomInterests());

        // need to wait for it to draw the aggregate sprite
        var (texture, colours) = await faceGenerator.GenerateAsync(newHuman.Data);
        newHuman.Face.Texture = texture;
        newHuman.ColorFace = colours[0];
        newHuman.ColorHair = colours[1];
        newHuman.ColorClothes = colours[2];
        newHuman.ColorTrim = colours[3];
        newHuman.ColorEyes = colours[4];
        newHuman.ColorPhone = colours[5];
        newHuman.Phone.Modulate = colours[5];
    }

    private string GetRandomName(HumanPersonalData.Gender gender)
    {
        string firstName;
        if (gender == HumanPersonalData.Gender.Male)
        {
            firstName = gameData.MaleFirstNames.values.GetRandom();
        }
        else if (gender == HumanPersonalData.Gender.Female)
        {
            firstName = gameData.FemaleFirstNames.values.GetRandom();
        }
        else
        {
            firstName = gameData.MaleFirstNames.values.GetRandom();
        }
        string lastName = gameData.SecondNames.values.GetRandom();
        return $"{firstName} {lastName}";
    }

    private static int GetRandomHeight(HumanPersonalData.Gender gender)
    {
        (int min, int max) = (155, 210);
        double mean, stdDev;

        if (gender == HumanPersonalData.Gender.Male)
        {
            mean = 177.0;
            stdDev = 8.0;
        }
        else if (gender == HumanPersonalData.Gender.Female)
        {
            mean = 164.0;
            stdDev = 7.0;
        }
        else
        {
            mean = 170.0;
            stdDev = 15.0;
        }

        return GenerateNormallyDistributedHeight(min, max, mean, stdDev);
    }

    private static int GenerateNormallyDistributedHeight(int min, int max, double mean, double stdDev)
    {
        double height;
        do
        {
            double u1;
            do {
                u1 = GD.Randf();
            } while (u1 <= double.Epsilon);

            double u2 = GD.Randf();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            height = mean + stdDev * randStdNormal;
        } while (height < min || height > max);

        return (int)Math.Round(height);
    }

    private static HumanPersonalData.Gender GetRandomGender()
    {
        return Random.Shared.NextEnum<HumanPersonalData.Gender>();
    }

    private static HumanPersonalData.Orientation GetRandomOrientation()
    {
        return Random.Shared.NextEnum<HumanPersonalData.Orientation>();
    }

    private Nationality GetRandomNationality()
    {
        return gameData.Nationalities.GetRandom();
    }

    private ContentType[] GetRandomInterests()
    {
        var pool = gameData.ContentTypes;
        var shuffled = new ContentType[pool.Length];
        pool.CopyTo(shuffled, 0);
        int count = GD.RandRange(4, Math.Min(6, shuffled.Length));
        var result = new ContentType[count];
        for (int i = 0; i < count; i++)
        {
            int index = GD.RandRange(i, shuffled.Length - 1);
            (shuffled[i], shuffled[index]) = (shuffled[index], shuffled[i]);
            result[i] = shuffled[i];
        }
        return result;
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
