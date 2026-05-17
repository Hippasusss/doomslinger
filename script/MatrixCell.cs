using Godot;

namespace DoomSlinger;

public partial class MatrixCell : PanelContainer
{
    [Export] private Button button;
    [Export] private RichTextLabel text;

    public Button Button {get => button;}
    public RichTextLabel Text {get => text;}

    private Tween colorTween;

    public Color Color
    {
        get => button.SelfModulate;
        set
        {
            colorTween?.Kill();
            colorTween = CreateTween();
            colorTween.TweenProperty(button, "self_modulate", value, 0.5f);
        }
    }
}
