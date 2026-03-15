









using Godot;
using System;
using Utils;

public partial class HumanDataDisplay : Sprite2D
{
    [Export] private IDDisplay idDisplay;
    [Export] private HeartMonitor heartMonitor;
    private Human currentHuman;
    private DeltaTimer updateTimer = new (0.2);

    public override void _Process(double delta)
    {
        if(updateTimer.Delta(delta))
        {
            UpdateCurrentHumanData();
        }
    }

    public void DisplayNewHuman(Human newHuman)
    {
        currentHuman = newHuman;
        idDisplay.UpdateID(newHuman);
        UpdateCurrentHumanData();
        heartMonitor.Wipe();
    }

    public void ClearDisplay()
    {

    }

    private void UpdateCurrentHumanData()
    {
        if(currentHuman != null)
        {
            heartMonitor.SetBPM(currentHuman.stats.rage);
        }
    }



}
