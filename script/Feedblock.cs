










using Godot;
using System;

public partial class Feedblock : Sprite2D
{
	public HumanStats stats;

    public override void _Ready()
    {
        // Set a unique seed for the wander pattern in the shader
        var seed = new Vector2(GD.Randf() * 100.0f, GD.Randf() * 100.0f);
        RenderingServer.CanvasItemSetInstanceShaderParameter(GetCanvasItem(), "seed_offset", seed);
    }

    public void SetColour(float r, float g, float b, float a)
    {
        var newColour = new Color(r,g,b,a);
        Modulate = newColour;
    }

    public void SetColour(Color color)
    {
        Modulate = color;
    }
}
