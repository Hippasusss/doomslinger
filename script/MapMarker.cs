using Godot;
using System;

public partial class MapMarker : Node2D
{
    [Signal] public delegate void MovementFinishedEventHandler();

    [Export] private NavigationAgent2D navigationAgent;
    [Export] private Sprite2D markerEmblem;
    [Export(PropertyHint.Range, "1,300,1")] private float moveSpeed = 10.0f;
    [Export(PropertyHint.Range, "0.1,20,0.1")] private float pathDesiredDistance = 1.0f;
    [Export(PropertyHint.Range, "0.1,20,0.1")] private float targetDesiredDistance = 1.0f;
    [Export] private Color selectedColor = Colors.White;
    [Export] private Color unselectedColor = Colors.Gray;
    [Export(PropertyHint.Range, "1,2,0.01")] private float selectedPulseScale = 1.4f;
    [Export(PropertyHint.Range, "0.1,2,0.01")] private float pulseDuration = 0.5f;
    private Tween pulseTween;
    private Vector2 baseScale = Vector2.One;
    private Vector2[] currentPath = [];
    private int currentPathIndex = 0;

    public override void _Ready()
    {
        if(navigationAgent != null)
        {
            navigationAgent.PathDesiredDistance = pathDesiredDistance;
            navigationAgent.TargetDesiredDistance = targetDesiredDistance;
        }

        baseScale = markerEmblem.Scale;
        SetSelected(false);
    }

    public override void _PhysicsProcess(double delta)
    {
        if(!IsMoving) return;

        Vector2 nextPathPosition = currentPath[currentPathIndex];
        Position = Position.MoveToward(nextPathPosition, moveSpeed * (float)delta);

        if(!Position.IsEqualApprox(nextPathPosition) && Position.DistanceTo(nextPathPosition) > 0.05f) return;

        Position = nextPathPosition;
        currentPathIndex++;
        while(currentPathIndex < currentPath.Length && Position.DistanceTo(currentPath[currentPathIndex]) <= 0.05f)
        {
            currentPathIndex++;
        }

        if(currentPathIndex < currentPath.Length) return;

        currentPath = [];
        currentPathIndex = 0;
        EmitSignal(SignalName.MovementFinished);
    }

    public void SetPath(Vector2[] path)
    {
        currentPath = path;
        currentPathIndex = 0;

        while(currentPathIndex < currentPath.Length && Position.DistanceTo(currentPath[currentPathIndex]) <= 0.05f)
        {
            currentPathIndex++;
        }

        if(currentPathIndex < currentPath.Length) return;

        currentPath = [];
        currentPathIndex = 0;
        EmitSignal(SignalName.MovementFinished);
    }

    public void SetSelected(bool selected)
    {
        pulseTween?.Kill();
        pulseTween = null;

        markerEmblem.Modulate = selected ? selectedColor : unselectedColor;
        markerEmblem.Scale = baseScale;

        if(!selected) return;

        pulseTween = CreateTween();
        pulseTween.SetLoops();
        pulseTween.TweenProperty(markerEmblem, "scale", baseScale * selectedPulseScale, pulseDuration)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.InOut);
        pulseTween.TweenProperty(markerEmblem, "scale", baseScale, pulseDuration)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.InOut);
    }

    public NavigationAgent2D NavigationAgent => navigationAgent;
    public bool IsMoving => currentPathIndex < currentPath.Length;
}
