using Godot;
using System.Collections.Generic;

public class HumanStats
{
    public Stat dopamine;
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

    public HumanStats(float dopamine = 5, float rage = 0, float hunger = 0, float fatigue = 0, float fear = 0, float rate = 0)
    {
        this.dopamine = new("dopamine", dopamine, rate);
        this.rage = new("rage", rage, rate);
        this.fear = new("fear", fear, rate);
        this.hunger = new("hunger", hunger, rate);
        this.fatigue = new("fatigue", fatigue, rate);

        engagement = new("engagement", 5);
        longTermFatigue = new("fatigue", fatigue, rate * 100, targetStat: this.fatigue);
        addiction = new("addiction", 0);
        mentalStability = new("mental stability", 10);

        allStats.AddRange([this.dopamine, this.rage, this.fear, this.hunger, this.fatigue, engagement, longTermFatigue, addiction, mentalStability]);
        mainStats.AddRange([this.dopamine, this.rage, this.fear, this.hunger, this.fatigue]);
        deepStats.AddRange([engagement, longTermFatigue, addiction, mentalStability]);
    }

    public static HumanStats operator + (HumanStats a, HumanStats b)
    {
        a.rage += b.rage;
        a.dopamine += b.dopamine;
        a.fear += b.fear;
        a.hunger += b.hunger;
        a.fatigue += b.fatigue;
        a.engagement.Value = Mathf.Max(a.rage, a.fear) * a.fatigue.GetNormalised();
        return a;
    }

    public void RandomizeStats(float rangeLow, float rangeHigh)
    {
        dopamine.Randomize(rangeLow, rangeHigh);
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

    public void UpdateAll(double delta)
    {
        foreach(Stat s in allStats)
        {
            s.Update(delta);
        }
    }
}

