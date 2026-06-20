using Godot;

namespace DoomSlinger;

public partial class ColoredLabel : Label
{
    [Export] private Panel background;

    public void SetBackgroundColour(Color color)
    {
        background.SelfModulate = color;
    }
}
