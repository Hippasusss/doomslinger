







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
    [Export] private Sprite2D warnignSymbol;
    private HumanStats stats = new();
    private HumanPersonalData data = new();
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
        stats = new(5);
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

    DeltaTimer warningTimerCheck = new(0.2);
    DeltaTimer exitTimer =  new(10);
    public override void _Process(double delta)
    {
        if(warningTimerCheck.Delta(delta))
        {
            if(stats.AreAnyOver(0.79f))
            {
                ToggleWarning(true);
            }
            else
            {
                ToggleWarning(false);
            }
            if(stats.AreAnyOver(0.99f))
            {
                SetUserOnline(false);
            }
        }
        if(!isOnline)
        {
            if(exitTimer.Delta(delta))
            {
                SetUserOnline(true);
            }
            stats.CoolDown(delta);
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

    private void ToggleWarning(bool onOff)
    {
        warnignSymbol.Visible = onOff;
    }

    public void OnMouseEnter()
    {
    }

    public void OnMouseExit()
    {
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


public class Stat
{
    private readonly string name;
    private float value;
    private float targetValue;
    private float coolValue;

    private float rate;
    private (float min, float max) range;

    public float Value 
    { 
        get {return this.value;} 
        set 
        {
            targetValue = value;
            if(rate == 0)
            {
                this.value = value;
            }
        } 
    }
    public string Name => name;


    public Stat(string name, float value = 0, float rate = 0, (float min, float max) range = default)
    {
        this.name = name;
        this.value = value;
        this.rate = rate;
        this.targetValue = value;
        this.coolValue = 0;
        this.range = range == (0,0) ? (0, 10) : range;
    }

    public static Stat operator + (Stat a, float b)
    {
        a.Value = Mathf.Clamp(a.value + b, a.range.min, a.range.max);
        return a;
    }

    public static Stat operator + (Stat a, Stat b)
    {
        return a + b.value;
    }


    public static implicit operator float(Stat b)
    {
        return b.Value;
    }

    public void Randomize(float min = 0, float max = 0)
    {
        if (min == 0 && max == 0)
        {
            Value = (float)GD.RandRange(range.min, range.max);
        }
        else
        {
            range.min = min;
            range.max = max;
            Value = (float)GD.RandRange(min,max);
        }
    }

    public float GetNormalised()
    {
        
        return range.max == 0 ?  0 : value/range.max;
    }

    public bool IsOver(float percent)
    {
        if(GetNormalised() > percent) return true;
        else return false;
    }

    public void Update(float delta)
    {
        float t = 1.0f - Mathf.Exp(-rate * delta);
        value = Mathf.Lerp(value, targetValue, t);
    }
     
    public void CoolDown(double delta)
    {
        value = Mathf.Lerp(value, coolValue, (float)delta);
        GD.Print(name);
        GD.Print(value);
        GD.Print(coolValue);
    }
}

public class HumanStats
{
    public Stat mood;
    public Stat rage;
    public Stat fear;
    public Stat hunger;
    public Stat fatigue;

    public Stat engagement;
    public Stat longTermFatigue;
    public Stat addiction;
    public Stat mentalStability;

    public readonly List<Stat> mainStats = [];
    public readonly List<Stat> deepStats = [];
    public readonly List<Stat> allStats = [];

    public HumanStats(float mood = 5, float rage = 0, float hunger = 0, float fatigue = 0, float fear = 0)
    {


        this.mood = new("mood", mood);
        this.rage = new("rage", rage);
        this.fear = new("fear", fear);
        this.hunger = new("hunger", hunger);
        this.fatigue = new("fatigue", fatigue);

        engagement = new("engagement", 5);
        longTermFatigue = new("fatigue", fatigue);
        addiction = new("addiction", 10);
        mentalStability = new("mental stability", 10);
        allStats.AddRange([this.mood, this.rage, this.fear, this.hunger, this.fatigue, engagement, longTermFatigue, addiction, mentalStability]);
        mainStats.AddRange([this.mood, this.rage, this.fear, this.hunger, this.fatigue]);
        deepStats.AddRange([engagement, longTermFatigue, addiction, mentalStability]);
    }

    public static HumanStats operator + (HumanStats a, HumanStats b)
    {
        HumanStats newStats = a;
        newStats.rage += b.rage;
        newStats.mood += b.mood;
        newStats.fear += b.fear;
        newStats.hunger += b.hunger;
        newStats.fatigue += b.fatigue;
        newStats.engagement.Value = Mathf.Max(newStats.rage, newStats.fear) * newStats.fatigue.GetNormalised();
        return newStats;
    }

    public void RandomizeStats(float rangeLow = - 2, float rangeHigh = 2)
    {
        mood.Randomize(rangeLow, rangeHigh);
        rage.Randomize(rangeLow, rangeHigh);
        fear.Randomize(rangeLow, rangeHigh);
        hunger.Randomize(rangeLow, rangeHigh);
        fatigue.Randomize(rangeLow, rangeHigh);
    }

    public bool AreAnyOver(float percent)
    {
        foreach(Stat s in mainStats)
        {
            if(s.IsOver(percent)) return true;
        }
        return false;
    }

    public void CoolDown(double delta)
    {
        foreach(Stat s in mainStats)
        {
            s.CoolDown(delta);
        }
    }
}
