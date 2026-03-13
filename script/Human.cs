







using Godot;

public partial class Human : Node2D
{
	[Export]private Feed feed;
	private string name = "Danny";
	private HumanStats stats;

	public override void _Ready()
	{
		feed.feedCallback += ReadFeedBlock;
	}

	private void ReadFeedBlock(Feedblock block)
	{
		stats += block.stats;
		GD.Print(stats.mood);
	}
}

public struct HumanStats(int mood = 5, int attention = 10, int rage = 0, int hunger = 0, int fatigue = 0)
{
	public int mood = mood;
	public int attention = attention;
	public int rage = rage;
	public int hunger = hunger;
	public int fatigue = fatigue;

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

	public void RandomizeStats()
	{
		mood = GD.RandRange(0,10);
		attention = GD.RandRange(0,10);
		rage = GD.RandRange(0,10);
		hunger = GD.RandRange(0,10);
		fatigue = GD.RandRange(0,10);
	}
}
