using Godot;
using System;

public partial class TextTreeObject : RichTextLabel 
{
    [Signal] private delegate void EndOfTextEventHandler();

    [Export]private string[] textList = [];
    private int index = 0;
    public override void _Ready()
    {
        SetText(textList[0], 0f);
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
            index = 0;
            SetText(textList[0], 0.5f);
            EmitSignal(SignalName.EndOfText);
            return false;
        }

        SetText(textList[index]);
        return true;
    }

    public void OnClick(Node viewport, InputEvent clickEvent, long shape_idx)
    {
        if(!Visible) 
        {
            return;
        }
        if (clickEvent is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            if (mouseEvent.ButtonIndex == MouseButton.Left)
            {
                ShowNext();
            }
        }
    }
}
