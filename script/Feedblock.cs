using Godot;

namespace DoomSlinger;

public partial class Feedblock : Node2D
{
    [Export] private Sprite2D colourSprite;
    private BlockData blockData;
    public BlockData BlockData {
        get => blockData;
        set
        {
            blockData = value;
            SetColour(blockData.BlockColor);
        }
    }

    public override void _Ready()
    {
        Visible = false;
    }

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

    public void Reset(BlockData newBlockData)
    {
        if (newBlockData != null)
        {
            BlockData = newBlockData;
            Visible = true;
        }
        else
        {
            Visible = false;
        }
    }
}
