










using Godot;

public partial class Feedblock : Node2D 
{
    public HumanStats stats = new(0);
    [Export] private Sprite2D colourSprite;

    public void SetColour(float r, float g, float b, float a)
    {
        var newColour = new Color(r,g,b,a);
        colourSprite.Modulate = newColour;
    }

    public void SetColour(Color color)
    {
        colourSprite.Modulate = color;
    }

    public Color GetColour()
    {
        return colourSprite.Modulate;
    }

}
