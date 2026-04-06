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
            human.SetUserOnline(onOff);
        }
    }

    public void OnDayEnd()
    {
        GD.Print("dayendhumanmanager");
        SetAllOfflineState(false);
    }

    public void OnDayBegin()
    {
        GD.Print("daybeginhumanmanager");
        SetAllOfflineState(true);
    }

    public void OnHumanSelected(Human human)
    {
        foreach(Human otherHuman in humans)
        {
            if(human == otherHuman) continue;
            otherHuman.DeSelect();
        }
    }
}
