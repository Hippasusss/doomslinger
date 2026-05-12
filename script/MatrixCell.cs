using Godot;

namespace DoomSlinger;

public partial class MatrixCell : PanelContainer
{

    [Export] private Button button;
    [Export] private RichTextLabel text;

    public Button Button {get => button;}
    public RichTextLabel Text {get => text;}
    public Color Color
    {
        get
        {
            return button.SelfModulate;
        }
        set
        {
            button.SelfModulate=value;
        }
    }
}
