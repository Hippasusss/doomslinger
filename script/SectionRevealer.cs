using Godot;
using System;

public partial class SectionRevealer : Button 
{

    [Export] public RigidBody2D rigidBody2D;
    [Export] public AnimationPlayer animationPlayer;
    [Export] public Sprite2D parentSprite;
    [Export] public bool directionRight = true;
    [Export] public Curve tweenCurve;
    private bool open = false;

    private const int force = 20000;
    private const string animationCloseName = "close";
    private const string animationOpenName = "open";
    private const float tweenDuration = 0.5f;
    
    public override void _Ready()
    {
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
        else if (parentSprite != null)
        {
            float width = parentSprite.GetRect().Size.X;
            float movement = width * direction; 
            Tween openCloseTween = CreateTween();
            Callable tweenCurveCallable = Callable.From<float, float>(tweenCurve.SampleBaked);
            openCloseTween.TweenProperty(parentSprite, "position:x", movement, tweenDuration)
                .AsRelative() 
                .SetCustomInterpolator(tweenCurveCallable);
        }
        open = shouldOpen;
    }
}
