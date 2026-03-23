







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

    // private DeltaTimer tempTimerOnOffTest = new(20,30);
    // public override void _Process(double delta)
    // {
    //     if(tempTimerOnOffTest.Delta(delta))
    //     {
    //         SetUserOnline(!isOnline);
    //     }
    //
    // }

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

public struct HumanStats(float mood = 5, float engagement = 10, float rage = 0, float hunger = 0, float fatigue = 0, float fear = 0)
{
    public float mood = mood;
    public float rage = rage;
    public float fear = fear;
    public float hunger = hunger;
    public float fatigue = fatigue;

    public float engagement = engagement;
    public float longTermFatigue = fatigue;
    public float addiction = engagement;
    public float mentalStability = 10;

    public static HumanStats operator + (HumanStats a, HumanStats b)
    {
        const int min = 0;
        const int max = 10;
        return new HumanStats(
                Mathf.Clamp(a.mood + b.mood, min, max),
                Mathf.Clamp(a.rage + b.rage, min, max),
                Mathf.Clamp(a.fear + b.fear, min, max),
                Mathf.Clamp(a.hunger + b.hunger, min, max),
                Mathf.Clamp(a.fatigue + b.fatigue, min, max)
                );
    }

    public void RandomizeStats(int rangeLow = - 2, int rangeHigh = 2)
    {
        mood = GD.RandRange(rangeLow, rangeHigh);
        rage = GD.RandRange(rangeLow, rangeHigh);
        fear = GD.RandRange(rangeLow, rangeHigh);
        hunger = GD.RandRange(rangeLow, rangeHigh);
        fatigue = GD.RandRange(rangeLow, rangeHigh);
    }


    private void CalculateLongTemStat(ref float longtermStat, float shortTermStat)
    {
        const float rate = 10;
        longtermStat = (shortTermStat - longtermStat) / rate;
    }






}
