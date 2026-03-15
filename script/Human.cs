







using Godot;
using System;
using System.Collections.Generic;

public partial class Human : Node2D
{
	public HumanDataDisplay display;
	[Export] private Sprite2D face;
	[Export] private Feed feed;
	[Export] private Light2D light;
	public HumanStats stats;
	public HumanPersonalData data;

	public Sprite2D Face { get => face; set => face = value; }

	public override void _Ready()
	{
		feed.newMainFeedBlockCallBack += ReadFeedBlock;
	}

	private void ReadFeedBlock(Feedblock block)
	{
		stats += block.stats;
	}

	public void SetLightOnOff(bool onOff)
	{
		light.Enabled = onOff;
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

public struct HumanPersonalData
{
	public string name;
	public DateOnly DOB;
	public int height;
	public string gender;
	public string nationality;
	public int UID;
	private readonly static List<int> UIDs = [];

	public HumanPersonalData(string name,
									DateOnly DOB,
									int height,
									string gender,
									string nationality)
	{
		this.name = name;
		this.DOB = DOB;
		this.height = height;
		this.gender = gender;
		this.nationality = nationality;
		do
		{
			UID = GD.RandRange(0,99999);
		}
		while(UIDs.Contains(UID));
		UIDs.Add(UID);
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
		const int rangeLow = -5;
		const int rangeHigh = 10;
		mood = GD.RandRange(rangeLow, rangeHigh);
		attention = GD.RandRange(rangeLow, rangeHigh);
		rage = GD.RandRange(rangeLow, rangeHigh);
		hunger = GD.RandRange(rangeLow, rangeHigh);
		fatigue = GD.RandRange(rangeLow, rangeHigh);
	}
}
