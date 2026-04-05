using Godot;
using System;

public partial class BlackoutController : Node2D
{

    [Export] private Sprite2D blackout;
    [Export] private TextTreeObject textTree;
    private bool enabled = true;
    private const float transitionSpeed = 1f;
    private const float dayLength = 20;

    private readonly Utils.DeltaTimer timer = new(dayLength);
    public override void _Process(double delta)
    {
        if(enabled) return;
        if (timer.Delta(delta))
        {
            ToggleBlackout(true);
        }
    }

    public void ToggleBlackout(bool enable)
    {
        Tween tween = CreateTween();
        if(enable) 
        { 
            Visible = true;
            tween.TweenProperty(blackout, "modulate:a", 1f, transitionSpeed);
            textTree.ShowNext();
        }
        else
        {
            tween.TweenProperty(blackout, "modulate:a", 0f, transitionSpeed);
        }
        tween.TweenCallback(Callable.From(() => {Visible = enable; enabled = enable;}));
    }

    public void Continue()
    {
        if(!textTree.ShowNext())
        {
            ToggleBlackout(false);
        }
    }

    public void OnClick(Node viewport, InputEvent clickEvent, long shape_idx)
    {
        if(!enabled) 
        {
            return;
        }
        if (clickEvent is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            if (mouseEvent.ButtonIndex == MouseButton.Left)
            {
                Continue();
            }
        }
    }
}
