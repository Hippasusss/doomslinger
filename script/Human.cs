







using Godot;

public partial class Human : Node2D
{
	[Export] private Feed feed;
	[Export] private Light2D light;
	private string name = "Danny";
	private HumanStats stats;

	public override void _Ready()
	{
		feed.newMainFeedBlockCallBack += ReadFeedBlock;
	}

	private void ReadFeedBlock(Feedblock block)
	{
		stats += block.stats;
		GD.Print(stats.mood);
	}

	private void ToggleLight(bool on)
	{
		light.Enabled = on;
	}

	public void Select()
	{
		display.DisplayNewHuman(this);
	}

	public void onClick(Node viewport, InputEvent clickEvent, long shape_idx)
	{
		if (clickEvent is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
		{
			if (mouseEvent.ButtonIndex == MouseButton.Left)
			{
				Select();
			}
		}
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
