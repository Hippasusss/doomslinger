using Godot;
using Utils;

public partial class DayNightTimer : Node2D
{
    [Signal] public delegate void DayEndEventHandler();
    [Signal] public delegate void DayBeginEventHandler();

    [Export] public float dayLengthMins = 3;

    private readonly DeltaTimer dayTimer = new(3*60);

    public override void _Ready()
    {
        dayTimer.SetResetTime(dayLengthMins * 60, true);
    }

    public override void _Process(double delta)
    {
        if(dayTimer.Delta(delta))
        {
            EndDay();
        }
    }

    private void BeginDay()
    {
        dayTimer.Reset();
        dayTimer.Start();
        EmitSignal(SignalName.DayBegin);
    }

    private void EndDay()
    {
        dayTimer.Stop();
        EmitSignal(SignalName.DayEnd);
    }

    public void OnBlackoutFinished(bool enable)
    {
        if(!enable) BeginDay();
    }

}
