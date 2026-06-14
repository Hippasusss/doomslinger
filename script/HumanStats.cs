using System.Collections.Generic;
using Godot;

namespace DoomSlinger;

public class HumanStats
{
    public Stat Dopamine { get; set; }
    public Stat Seratonin { get; set; }
    public Stat Cortisol { get; set; }
    public Stat Ocytocin { get; set; }
    public Stat Melatonin { get; set; }

    public Stat Engagement { get; set; }
    public Stat LongTermFatigue { get; set; }
    public Stat MentalStability { get; set; }
    public Stat PoliticalLeaning { get; set; }
    public Stat AttentionSpan { get; set; }

    public readonly List<Stat> hormones = [];
    public readonly List<Stat> deepStats = [];
    public readonly List<Stat> allStats = [];


    public HumanStats(float newDopamine = 5, float newSeratonin = 0, float newCortisol = 0, float newOxytocin = 0, float newMelatonin = 0, float rate = 0)
    {
        Dopamine = new("dopamine", newDopamine, rate); // happy sad
        Seratonin = new("rage", newSeratonin, rate); // angry calm
        Cortisol = new("fear", newCortisol, rate); // scared confident
        Ocytocin = new("hunger", newOxytocin, rate); // love lonliness
        Melatonin = new("fatigue", newMelatonin, rate); // tired awake

        Engagement = new("engagement", 5); // Tend towards each block's human defined engagement score. Target changes each time a block is read (synonymous with addiction)
        LongTermFatigue = new("fatigue", newOxytocin, rate * 100, newTargetStat: Melatonin); // Tends towards melatonin level. 
        MentalStability = new("mental stability", 10); // certain videos decay this
        PoliticalLeaning = new("political leaning", 0, newRange: (-1, 1)); // Tends towards each current video if within some range of current human's political leaning. If very far away then it repels them. 

        hormones.AddRange([Dopamine, Seratonin, Cortisol, Ocytocin, Melatonin]);
        deepStats.AddRange([Engagement, LongTermFatigue, MentalStability, PoliticalLeaning]);
        allStats.AddRange(hormones);
        allStats.AddRange(deepStats);
    }

    public bool AreAnyOver(float percent)
    {
        foreach(Stat s in hormones)
        {
            if(s.IsOver(percent)) return true;
        }
        return false;
    }

    public void CoolDown(double delta)
    {
        foreach(Stat s in hormones)
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
