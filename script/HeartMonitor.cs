using Godot;
using System;
using Utils;

public partial class HeartMonitor : Node2D
{
    [Export] private Line2D line;
    [Export] private RichTextLabel BPMText;
    private const int numPoints = 300;
    private float targetValue = 0;
    private float amplitude = 15f;
    private float beepLength = 0.1f;
    private readonly DeltaTimer updateTimer = new(0.01);
    private readonly DeltaTimer beepTimer = new(2);
    private Tween beepTween;

    public override void _Ready()
    {
        SetBPM(40);
        Vector2 startPoint = line.Points[0];
        Vector2 endPoint = line.Points[1];
        endPoint.Y = startPoint.Y;
        line.RemovePoint(1);
        for(int i = 1; i < numPoints; i++)
        {
            Vector2 position = new((endPoint.X - startPoint.X) / numPoints * i, startPoint.Y);
            line.AddPoint(position, i);
        }
    }


    public override void _Process(double delta)
    {

        if(updateTimer.Delta(delta))
        {
            line.SetPointPosition(numPoints - 1, new(line.GetPointPosition(numPoints-1).X, targetValue));  
            for(int i = 1; i < numPoints - 1; i++)
            {
                line.SetPointPosition(i, new(line.GetPointPosition(i).X, line.GetPointPosition(i + 1).Y));
            }
        }
        if(beepTimer.Delta(delta))
        {
            AddBeep();
        }
    }

    private void AddBeep()
    {
        beepTween = GetTree().CreateTween();
        beepTween.TweenProperty(this, "targetValue", -amplitude, beepLength/2)
                .SetTrans(Tween.TransitionType.Quart)
                .SetEase(Tween.EaseType.In);
        beepTween.TweenProperty(this, "targetValue", amplitude/1.5f, beepLength)
                .SetTrans(Tween.TransitionType.Quart)
                .SetEase(Tween.EaseType.Out);
        beepTween.TweenProperty(this, "targetValue", 0.0f, beepLength)
                .SetTrans(Tween.TransitionType.Quart)
                .SetEase(Tween.EaseType.OutIn);
    }

    public void SetBPM(int newBPM)
    {
        if (newBPM < 5) return;
        beepTimer.SetResetTime(60f/newBPM);
        BPMText.Text = newBPM.ToString();
    }

    public void Wipe()
    {
        beepTween?.Kill();
        targetValue = 0;
        for(int i = 1; i < numPoints; i++)
        {
            line.SetPointPosition(i, new(line.GetPointPosition(i).X, 0));
        }
    }
}
