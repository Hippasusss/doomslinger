using Godot;
using System;

public partial class TextTreeObject : RichTextLabel 
{
    [Signal] private delegate void EndOfTextEventHandler();

    [Export]private string[] textList = [];
    private int index = 0;
    private bool enabled = true;

    public override void _Ready()
    {
        enabled = Visible;
    }

    private void SetText(string text, float speed = 0.2f)
    {
        Tween tween = CreateTween();
        tween.TweenProperty(this, "modulate:a", 0, speed);
        tween.TweenCallback(Callable.From(() => Text = text));
        tween.TweenProperty(this, "modulate:a", 1, speed);
    }

    public bool ShowNext()
    {
        index += 1;
        if(index >= textList.Length) 
        {
            enabled = false;
            index = 0;
            SetText("");
            EmitSignal(SignalName.EndOfText);
            return false;
        }
        SetText(textList[index]);
        return true;
    }

    public void OnClick(Node viewport, InputEvent clickEvent, long shape_idx)
    {
        if(!Visible || !enabled) { return; }
        if (clickEvent is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            if (mouseEvent.ButtonIndex == MouseButton.Left)
            {
                ShowNext();
            }
        }
    }

    public void OnBlackOutFinished(bool enable)
    {
        enabled = enable;
        if(enable) SetText(textList[index]);
    }
}
