using Godot;
using System;

public partial class BlackoutController : Node2D
{

    [Signal] public delegate void BlackoutFinishedEventHandler(bool start);

    [Export] private Sprite2D blackout;
    private const float transitionSpeed = 2f;

    public override void _Ready()
    {
        if(!Visible) ToggleBlackout(false, 0f);
    }

    public void ToggleBlackout(bool enable, float speed = transitionSpeed)
    {
        Tween tween = CreateTween();
        if(enable) 
        { 
            Visible = true;
            tween.TweenProperty(blackout, "modulate:a", 1f, transitionSpeed);
        }
        else
        {
            tween.TweenProperty(blackout, "modulate:a", 0f, transitionSpeed);
        }
        tween.TweenCallback(Callable.From(() => {Visible = enable;}));
        tween.TweenCallback(Callable.From(() => {EmitSignal(SignalName.BlackoutFinished, enable);}));
    }

    public void OnDayEnd()
    {
        ToggleBlackout(true);
    }

    public void OnTextFinishedSignal()
    {
        ToggleBlackout(false);
    }
}
