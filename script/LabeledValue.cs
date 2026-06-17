using Godot;

namespace DoomSlinger;

[Tool]
public partial class LabeledValue : Control
{
    [Export] private RichTextLabel Label;
    [Export] private RichTextLabel Value;

    private string _labelText;
    private string _valueText;

    [Export]
    public string LabelText
    {
        get => _labelText;
        set
        {
            _labelText = value;
            if (IsInsideTree()) CallDeferred(nameof(Sync));
        }
    }

    [Export]
    public string ValueText
    {
        get => _valueText;
        set
        {
            _valueText = value;
            if (IsInsideTree()) CallDeferred(nameof(Sync));
        }
    }

    public override void _Ready()
    {
        Sync();
    }

    private void Sync()
    {
        if (Label != null) Label.Text = _labelText;
        if (Value != null) Value.Text = _valueText;
    }
}
