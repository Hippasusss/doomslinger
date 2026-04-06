using Godot;
using System.Collections.Generic;

public partial class HumanManager : Node2D
{
    private List<Human> humans = [];
    public int HumanCount => humans.Count;

    public void AddHuman(Human human)
    {
        humans.Add(human);
        human.HumanSelected += OnHumanSelected;
    }

    public void SetAllOfflineState(bool onOff)
    {
        foreach(Human human in humans)
        {
            human.SetUserOnline(onOff, false);
        }
    }

    public void OnDayEnd()
    {
        SetAllOfflineState(false);
    }

    public void OnDayBegin()
    {
        SetAllOfflineState(true);
    }

    public void OnHumanSelected(Human human)
    {
        foreach(Human otherHuman in humans)
        {
            if(human == otherHuman) continue;
            otherHuman.Select(false);
        }
    }
}
