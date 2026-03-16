using Godot;
using System;

public partial class CameraController : Camera2D
{
    [Export] public float TransitionTime = 0.8f;

    private Tween _moveTween;


    public void MovePositionToNode(Human nodeToMoveTo)
    {
        if (nodeToMoveTo == null) return;
        if (_moveTween != null && _moveTween.IsRunning())
        {
            _moveTween.Kill();
        }

        float visibleWidth = GetViewportRect().Size.X / Zoom.X;
        float targetX = nodeToMoveTo.GlobalPosition.X - (visibleWidth / 3.0f);

        _moveTween = GetTree().CreateTween();

        _moveTween.SetTrans(Tween.TransitionType.Cubic);
        _moveTween.SetEase(Tween.EaseType.Out);

        _moveTween.TweenProperty(
                this, 
                "global_position:x", 
                targetX, 
                TransitionTime
                );
    }
}
