using Godot;
using System;

public partial class TextTreeObject : RichTextLabel 
{
    [Export]private string[] textList = [];
    private int index = -1;

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
            SetText("");
            index = -1;
            return false;
        }

        SetText(textList[index]);
        return true;
    }
}
