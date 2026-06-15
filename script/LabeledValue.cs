using Godot;

namespace DoomSlinger;

[Tool]
public partial class LabeledValue : Control
{
    [Export] private RichTextLabel Label;
    [Export] private RichTextLabel Value;

    [Export] public string LabelText
    {
        get => Label.Text;
        set => Label.Text = value;
    }

    [Export] public string ValueText
    {
        get => Value.Text;
        set => Value.Text = value;
    }

    public void Set(string label, string value)
    {
        LabelText = label;
        ValueText = value;
    }
}
