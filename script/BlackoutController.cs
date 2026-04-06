using Godot;
using System;

public partial class BlackoutController : Node2D
{

    [Signal] public delegate void BlackoutFinishedEventHandler();

    [Export] private Sprite2D blackout;
    private const float transitionSpeed = 1f;

    public void ToggleBlackout(bool enable)
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
        tween.TweenCallback(Callable.From(() => {if(!enable) EmitSignal(SignalName.BlackoutFinished);}));
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
