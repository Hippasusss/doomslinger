using System;
using Godot;

namespace DoomSlinger;

public class BlockData
{
    public Company Owner {get; set;}

    private readonly float[] stats = new float[5];
    public ref float PoliticalLeaning => ref stats[0];
    public ref float Outrage => ref stats[1];
    public ref float Interesting => ref stats[2];
    public ref float Length => ref stats[4];

    public Span<float> Stats => stats.AsSpan();

    public Color BlockColor {get; set;}

    public BlockData()
    {
        BlockColor = Colors.White;
        Randomize();
    }

    public BlockData(Company company)
    {
        Owner = company;

        PoliticalLeaning = Mathf.Clamp(
                (float)GD.Randfn(company.PoliticalLeaning, company.Chaos),
                -1f, 1f
                );

        Outrage = Mathf.Clamp(
                (float)GD.Randfn(0, company.Chaos / 3f),
                -company.Chaos, company.Chaos);

        // Gaussian Dist around point scaled by age of company.
        // TODO: allow for more random values out side of company age restrictions.
        // For example an old compnany should occasinally make an interesting video
        float interestingCenter = 1f / (1f + (company.Age / 10f));
        Interesting = Mathf.Clamp(
                (float)GD.Randfn(interestingCenter, interestingCenter * 0.3f),
                0f, 1f);

        // TODO make better
        Length = GD.RandRange(7, 35);
        CalculateColor();
    }

    public void Randomize()
    {
        var random = Random.Shared;
        for (int i = 0; i < stats.Length; i++)
        {
            stats[i] = (float)(random.NextDouble() * 2.0 - 1.0);
        }
        CalculateColor();
    }

    private void CalculateColor()
    {
        float t = (PoliticalLeaning + 1f) / 2f;
        BlockColor = Colors.Salmon.Lerp(Colors.AliceBlue, t);
    }
}
