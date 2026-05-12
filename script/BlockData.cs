using System;

public class BlockData(float politicalLeaning, float outrage, float interesting, float fun, float length) 
{

    private readonly float[] stats = [politicalLeaning, outrage, interesting, fun, length];

    public ref float PoliticalLeaning => ref stats[0];
    public ref float Outrage => ref stats[1];
    public ref float Interesting => ref stats[2];
    public ref float Fun => ref stats[3];
    public ref float Length => ref stats[4];

    public Span<float> Stats => stats.AsSpan();

    public BlockData() : this(0, 0, 0, 0, 0) { }

    public void Randomize()
    {
        var random = Random.Shared;
        for (int i = 0; i < stats.Length; i++)
        {
            stats[i] = (float)(random.NextDouble() * 2.0 - 1.0);
        }
    }
}
