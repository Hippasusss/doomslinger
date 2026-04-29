using Godot;

public partial class MapMarker : Node2D
{
    [Signal] public delegate void MovementFinishedEventHandler();

    [Export] private Sprite2D markerEmblem;
    [Export] private Color selectedColor = Colors.White;
    [Export] private Color unselectedColor = Colors.Gray;
    [Export(PropertyHint.Range, "1,2,0.01")] private float selectedPulseScale = 1.4f;
    [Export(PropertyHint.Range, "0.1,2,0.01")] private float pulseDuration = 0.5f;
    private NavigationAgent navigationAgent;
    private Tween pulseTween;

    public override void _Ready()
    {
        navigationAgent = new NavigationAgent();
        AddChild(navigationAgent);
        navigationAgent.MovementFinished += () => EmitSignal(SignalName.MovementFinished);

        SetSelected(false);
    }

    public override void _PhysicsProcess(double delta)
    {
        navigationAgent.ProcessMovement(this, delta);
    }

    public void SetPath(Vector2[] path)
    {
        navigationAgent.SetPath(path, Position);
    }

    public void SetSelected(bool selected)
    {
        pulseTween?.Kill();
        pulseTween = null;

        markerEmblem.Modulate = selected ? selectedColor : unselectedColor;
        markerEmblem.Scale = Vector2.One;

        if(!selected) return;

        pulseTween = CreateTween();
        pulseTween.SetLoops();
        pulseTween.TweenProperty(markerEmblem, "scale", Vector2.One * selectedPulseScale, pulseDuration)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.InOut);
        pulseTween.TweenProperty(markerEmblem, "scale", Vector2.One, pulseDuration)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.InOut);
    }

    public bool IsMoving => navigationAgent.IsMoving;
}
