using Godot;

namespace DoomSlinger;

public partial class InterestsDisplay : HFlowContainer, IDisplay
{
    [Export] private PackedScene coloredLabelScene;

    public bool Enabled { get; set; } = true;

    public void UpdateDisplay(Human human)
    {
        if (!Enabled) return;
        ClearLabels();
        foreach (ContentType type in human.Data.interests)
        {
            ColoredLabel label = coloredLabelScene.Instantiate<ColoredLabel>();
            AddChild(label);
            label.Text = type.Name;
            label.SetBackgroundColour(type.Category.Color);
        }
    }

    public void ToggleOnOff(bool onOff)
    {
        if (!onOff) ClearLabels();
        Enabled = onOff;
    }

    private void ClearLabels()
    {
        foreach (Node child in GetChildren())
            child.QueueFree();
    }
}
