










using Godot;
using System;

public partial class Feedblock : Sprite2D
{
	public HumanStats stats;

    public void SetColour(float r, float g, float b, float a)
    {
        var newColour = new Color(r,g,b,a);
        Modulate = newColour;
    }
}
