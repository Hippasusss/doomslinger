







using Godot;
using System;
using System.Collections.Generic;
using Utils;

public partial class Human : Node2D
{
    [Signal] public delegate void HumanSelectedEventHandler(Human selected);

    [Export] private Sprite2D face;
    [Export] private Feed feed;
    [Export] private Light2D light;
    [Export] private AnimationPlayer animation;
    [Export] private Sprite2D phone;
    private HumanStats stats;
    private HumanPersonalData data;
    private Color[] colors;
    private bool isOnline = true;

    private static List<Human> allHumans = [];
    private bool selected = false;

    public Sprite2D Face { get => face; set => face = value; }
    public HumanStats Stats { get => stats; set => stats = value; }
    public HumanPersonalData Data { get => data; set => data = value; }
    public Color[] Colors { get => colors; set => colors = value; }
    public Sprite2D Phone { get => phone; set => phone = value; }
    public Feed Feed { get => feed; set => feed = value; }
    public bool IsOnline { get => isOnline;}


    public override void _Ready()
    {
        Feed.newMainFeedBlockCallBack += ReadFeedBlock;
        allHumans.Add(this);
        HumanSelected += selected => {
            foreach(Human human in allHumans)
            {
                if(human == this) continue;
                human.DeSelect();
            };
        };
    }

    private DeltaTimer tempTimerOnOffTest = new(5,10);
    public override void _Process(double delta)
    {
        if(tempTimerOnOffTest.Delta(delta))
        {
            SetUserOnline(!isOnline);
        }

    }

    public void SetUserOnline(bool onOff)
    {
        isOnline = onOff;
        feed.ToggleOnOff(onOff);
        light.Enabled = onOff;
    }

    private void ReadFeedBlock(Feedblock block)
    {
        Stats += block.stats;
    }

    public void Select()
    {
        if(!selected)
        {
            animation.Play("hover");
            EmitSignal(SignalName.HumanSelected, this);
            selected = true;
        }
    }

    public void DeSelect()
    {
        if(selected)
        {
            selected = false;
            animation.PlayBackwards("hover");
        }
    }

    public void OnClick(Node viewport, InputEvent clickEvent, long shape_idx)
    {
        if (clickEvent is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            if (mouseEvent.ButtonIndex == MouseButton.Left)
            {
                Select();
            }
        }
    }

    public void OnMouseEnter()
    {
        if(!selected)
        {
            // animation.Play("hover");
        }
    }

    public void OnMouseExit()
    {
        if(!selected)
        {
            // animation.PlayBackwards("hover");
        }
    }

}

public struct HumanPersonalData
{
    public enum EyeColour
    {
        blue,
        brown,
        green
    };

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
    public int longTermFatigue = fatigue;

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

    public string GetStatsString()
    {
        const int spacing = -3;
        return $"{attention, spacing} {fatigue, spacing} {hunger, spacing} {mood, spacing} {rage, spacing}";
    }





}
