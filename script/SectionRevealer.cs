using Godot;
using System;

public partial class SectionRevealer : Node2D
{
    [Export] public RigidBody2D rigidBody2D;
    [Export] public AnimationPlayer animationPlayer;
    private bool open = false;

    private const int force = 20000;
    private const string animationCloseName = "close";
    private const string animationOpenName = "open";

    public void Toggle()
    {
        if(rigidBody2D != null)
        {
            int sign = open ? -1 : 1;
            rigidBody2D.ApplyCentralImpulse(new(force * sign,0));
            open = !open;
        }
        else if (animationPlayer != null)
        {
            string animationToPlay = open ? animationCloseName : animationOpenName;
            animationPlayer.Play(animationToPlay);
            open = !open;
        }
    }

    public void Close()
    {
        if(!open) return;
        if(rigidBody2D != null)
        {
            rigidBody2D.ApplyCentralImpulse(new(-force,0));
            open = !open;
        }
        else if (animationPlayer != null)
        {
            animationPlayer.Play(animationCloseName);
            open = !open;
        }
    }

    public void OnClick(Node viewport, InputEvent clickEvent, long shape_idx)
    {
        if (clickEvent is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            if (mouseEvent.ButtonIndex == MouseButton.Left)
            {
                Toggle();
            }
        }
    }


}
