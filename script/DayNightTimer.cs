using Godot;
using Utils;
using System;

public partial class DayNightTimer : Node2D
{
    private readonly DeltaTimer dayLength = new(3*60);
    public override void _Process(double delta)
    {
        if(dayLength.Delta(delta))
        {
            EndDay();
        }
    }

    private void EndDay()
    {
    }

    private void BeginDay()
    {
    }
}
