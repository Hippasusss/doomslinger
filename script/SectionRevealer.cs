using Godot;
using System;

public partial class SectionRevealer : Node2D
{
    [Export] public RigidBody2D rigidBody2D;
    private bool open = false;
    private const int force = 20000;

    public override void _Ready()
    {
    }

    public void Toggle()
    {
        int sign = open ? -1 : 1;
        rigidBody2D.ApplyCentralImpulse(new(force * sign,0));
        open = !open;
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
