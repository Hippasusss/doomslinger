









using Godot;
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

    public void DisplayHuman(Human newHuman)
    {
        currentHuman = newHuman;
        if(newHuman.IsOnline)
        {
            UpdateCurrentHumanData();
        }
    }

    public void ClearDisplay()
    {
        currentHuman = null;
        foreach(IDisplay display in displays)
        {
            display.ToggleOnOff(false);
        }
    }

    private void UpdateCurrentHumanData()
    {
        if(currentHuman == null) return;
        foreach(IDisplay display in displays)
        {
            if(display.Enabled != currentHuman.IsOnline) display.ToggleOnOff(currentHuman.IsOnline);
            if(currentHuman.IsOnline) display.UpdateDisplay(currentHuman);
        }
    }



}
