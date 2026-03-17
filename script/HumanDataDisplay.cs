









using Godot;
using System;
using Utils;

public partial class HumanDataDisplay : Sprite2D
{
    [Export] private IDDisplay idDisplay;
    [Export] private HeartMonitor heartMonitor;
    [Export] private Eye eye;
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
        eye.SetEyelidColor(newHuman.Colors[0]);
        eye.SetEyeColor(newHuman.Colors[4]);
        eye.MoveEyeball(4);
        eye.ScaleIris((float)GD.RandRange(0.5, 1.5), 0);
    }

    public void ClearDisplay()
    {

    }

    private void UpdateCurrentHumanData()
    {
        if(currentHuman != null)
        {
            heartMonitor.SetBPM(currentHuman.Stats.rage);
        }
    }



}
