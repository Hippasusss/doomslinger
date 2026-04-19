using Godot;
using System;

public partial class Selector : Panel
{
    [Export] Button upButton;
    [Export] Button downButton;
    [Export] Label label;
    private int currentValue = 0;
    private const int maxValue = 9;

    public int CurrentValue { 
        get { return currentValue; } 
        set 
        {
            currentValue = Mathf.Clamp(value, -maxValue, maxValue); 
            label.Text = currentValue.ToString();
        }
    }

    public override void _Ready()
    {
        label.Text = currentValue.ToString();
        upButton.Pressed += () => { CurrentValue += 1;};
        downButton.Pressed += () => { CurrentValue -= 1;};
    }
}
