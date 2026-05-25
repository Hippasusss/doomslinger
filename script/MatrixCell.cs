using Godot;

namespace DoomSlinger;

public partial class MatrixCell : Panel
{
    [Export] private Button button;
    [Export] private RichTextLabel text;
    [Export] private Control SelectionNumber;
    [Export] private AnimationPlayer animationPlayer;

    public Button Button {get => button;}
    public RichTextLabel Text {get => text;}
    private Panel border;
    private RichTextLabel numberText;

    public override void _Ready()
    {
        border = SelectionNumber.GetChild(0) as Panel; 
        numberText = SelectionNumber.GetChild(1) as RichTextLabel; 
    }
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

    public void SetColor(Color newColor, float tweenTime = 0)
    {
        colorTween?.Kill();
        colorTween = CreateTween();
        colorTween.TweenProperty(button, "self_modulate", newColor, tweenTime);
    }

    public void HideSelectionNumber(bool hide = true)
    {
        SelectionNumber.Visible = !hide;
    }

    public void SetSelectionNumber(int number, Color color)
    {
        numberText.Text = number.ToString();
        border.Modulate = color;
    }

    public void Flash(bool shouldFlash)
    {
        if(shouldFlash)
        {
            animationPlayer.Play("Flash");
        }
        else
        {
            animationPlayer.Stop();
        }
    }

    public void Reset()
    {
        colorTween?.Kill();
        button.SelfModulate = Colors.White;
        text.Text = "";
        HideSelectionNumber(true);
        animationPlayer.Stop();
    }
}
