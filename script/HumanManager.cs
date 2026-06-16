using System.Collections.Generic;
using Godot;

namespace DoomSlinger;

public partial class HumanManager : Node2D
{
    [Export] private CameraController camera;
    [Export] private HumanDataSection humanDataSection;
    [Export] private AlgoSection algoSection;
    [Export] private MapSection mapSection;
    private List<Human> humans = [];
    public int HumanCount => humans.Count;

    public void AddHuman(Human human)
    {
        mapSection.RegisterHumanOnMap(human);
        humans.Add(human);
        human.HumanSelected += OnHumanSelected;
        human.Feed.OnRequestBlockDataCallBack = () => algoSection.TryConsumeFirstBid(human)?.BlockData;
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
        camera.MovePositionToNode(human);
        mapSection.SetHumanToTrack(human);
        mapSection.SetHumanMarkerDestinationToRandomLocation(human);
        if(human.Selected)
        {
            humanDataSection.DisplayHuman(human);
            algoSection.DisplayHuman(human);
        }
        else humanDataSection.ClearDisplay();
        foreach(Human otherHuman in humans)
        {
            if(human == otherHuman) continue;
            otherHuman.Select(false, false);
        }
    }

}
