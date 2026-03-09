







using Godot;

public partial class Human : Node2D
{
	private string name = "Danny";
	private HumanStats stats;


	private void ReadFeedBlock(Feedblock block)
	{
		stats += block.stats;
	}
}

public readonly struct HumanStats(int mood = 5, int attention = 10, int rage = 0, int hunger = 0, int fatigue = 0)
{
	private readonly int mood = mood;
	private readonly int attention = attention;
	private readonly int rage = rage;
	private readonly int hunger = hunger;
	private readonly int fatigue = fatigue;

	public static HumanStats operator +(HumanStats a, HumanStats b)
	{
		return new HumanStats(
			a.mood + b.mood,
			a.attention + b.attention,
			a.rage + b.rage,
			a.hunger + b.hunger,
			a.fatigue + b.fatigue
		);
	}
}
