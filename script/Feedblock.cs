using Godot;

namespace DoomSlinger;

public partial class Feedblock : Node2D
{
    [Export] private Sprite2D colourSprite;
    [Export] private RichTextLabel priceText;
    [Export] private RichTextLabel lengthText;
    [Export] private RichTextLabel leaningText;
    [Export] private InterestsDisplay interestsDisplay;
    private BlockData blockData;
    public BlockData BlockData {
        get => blockData;
        set
        {
            blockData = value;
            SetColour(blockData.BlockColor);
            priceText.Text = $"${blockData.Price}";
            lengthText.Text = $"{blockData.Length:0}s";
            leaningText.Text = $"{blockData.PoliticalLeaning:0.0}";
            interestsDisplay.UpdateDisplay(blockData.ContentTypes);
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
