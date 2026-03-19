









using Godot;
using System;
using Utils;

public partial class HumanDataDisplay : Sprite2D
{
    [Export] private IDDisplay idDisplay;
    [Export] private HeartMonitorDisplay heartMonitorDisplay;
    [Export] private EyeDisplay eyeDisplay;
    [Export] private FeedDataDisplay feedDataDisplay;
    private IDisplay[] displays;
    private Human currentHuman;

    public override void _Ready()
    {
        displays = [idDisplay, heartMonitorDisplay, eyeDisplay, feedDataDisplay];
    }
    private readonly DeltaTimer updateTimer = new (0.2);
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
        if(newHuman.IsOnline)
        {
            UpdateCurrentHumanData();
            eyeDisplay.MoveEyeball(4);
            eyeDisplay.ScaleIris((float)GD.RandRange(0.5, 1.5), 0);
        }
    }

    private void UpdateCurrentHumanData()
    {
        if(currentHuman == null) return;
        if(currentHuman.IsOnline)
        {
            foreach(IDisplay display in displays)
            {
                if(display.Enabled != currentHuman.IsOnline) display.ToggleOnOff(currentHuman.IsOnline);
                display.UpdateDisplay(currentHuman);
            }
        }
        else
        {
            foreach(IDisplay display in displays)
            {
                if(display.Enabled != currentHuman.IsOnline) display.ToggleOnOff(currentHuman.IsOnline);
            }
        }
    }


}
