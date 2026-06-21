using Godot;

namespace DoomSlinger;

public partial class HumanDataSection : Panel, ISection
{
    [Export] private IDDisplay idDisplay;
    [Export] private HeartMonitorDisplay heartMonitorDisplay;
    [Export] private EyeDisplay eyeDisplay;
    [Export] private FeedDataDisplay feedDataDisplay;
    [Export] private WaveFormDisplay waveFormDisplay;
    [Export] private MapDisplay mapDisplay;
    [Export] private MapSection mapSection;
    [Export] private InterestsDisplay interestsDisplay;
    [Export] private SectionRevealer revealer;
    private IDisplay[] displays;
    private Human currentHuman;

    public void Toggle() => revealer?.Toggle();
    public void SetOpen(bool open) => revealer?.SetOpen(open);
    public bool IsOpen => revealer?.IsOpen ?? false;

    public override void _Ready()
    {
        displays = [idDisplay, heartMonitorDisplay, eyeDisplay, feedDataDisplay, waveFormDisplay, mapDisplay, interestsDisplay];
        ClearDisplay();
        mapDisplay.Init(mapSection);
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
