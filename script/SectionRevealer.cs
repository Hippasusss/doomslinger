using Godot;

namespace DoomSlinger;

public partial class SectionRevealer : Button 
{

    [Export] public RigidBody2D rigidBody2D;
    [Export] public AnimationPlayer animationPlayer;
    [Export] public Panel parentRect;
    [Export] public bool directionRight = true;
    [Export] public Curve tweenCurve;
    public bool IsOpen => open;
    private bool open = false;
    private float closedX;
    private Tween openCloseTween;

    private const int force = 20000;
    private const string animationCloseName = "close";
    private const string animationOpenName = "open";
    private const float tweenDuration = 0.5f;
    
    public override void _Ready()
    {
        parentRect ??= GetParent() as Panel;
        if(parentRect != null)
        {
            closedX = parentRect.Position.X;
        }
        
        Pressed += Toggle;
    }

    public void Toggle()
    {
        if(open)
        {
            SetOpen(false);
            return;
        }

        SetOpen(true);
    }

    public void SetOpen(bool shouldOpen)
    {
        if(open == shouldOpen) return;

        int direction = shouldOpen ? 1 : -1;
        direction *= directionRight ? 1 : -1;

        if(rigidBody2D != null)
        {
            rigidBody2D.ApplyCentralImpulse(new(force * direction,0));
        }
        else if (animationPlayer != null)
        {
            animationPlayer.Play(shouldOpen ? animationOpenName : animationCloseName);
        }
        else if (parentRect != null)
        {
            openCloseTween?.Kill();
            float width = parentRect.GetRect().Size.X;
            float offset = width * direction;
            float targetX = shouldOpen ? closedX + offset : closedX;
            openCloseTween = CreateTween();
            openCloseTween.TweenProperty(parentRect, "position:x", targetX, tweenDuration)
                .SetCustomInterpolator(Callable.From<float, float>(tweenCurve.SampleBaked));
        }
        open = shouldOpen;
    }
}
