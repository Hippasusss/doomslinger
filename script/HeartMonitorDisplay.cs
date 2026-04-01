using Godot;
using System;
using Utils;

public partial class HeartMonitorDisplay : Control, IDisplay
{
    [Export] private Line2D line;
    [Export] private RichTextLabel BPMText;
    private Human currentHuman;

    public bool Enabled {get; set;} = true;

    private const int numPoints = 300;
    private const float beepLength = 0.1f;
    private const float amplitude = 15f;
    private readonly (int min, int max) BPMRange = (35, 199);

    private float targetValue = 0;
    private readonly DeltaTimer updateTimer = new(0.01);
    private readonly DeltaTimer beepTimer = new(2);
    private Tween beepTween;

    public override void _Ready()
    {
        Enabled = true;
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

        if(!Enabled) return;
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

    private void SetBPM(int newBPM)
    {
        if (newBPM < 5) return;
        beepTimer.SetResetTime(60f/newBPM);
        BPMText.Text = newBPM.ToString();
    }

    private int CalculateBPM(Human human)
    {
        int BPMdiff = BPMRange.max- BPMRange.min;

        float rage = human.Stats.rage.GetNormalised();
        float fear = human.Stats.fear.GetNormalised();
        float fatigue = human.Stats.fatigue.GetNormalised();

        int rageBPM = (int)(BPMdiff * rage * (1 - fatigue * 0.5f) + BPMRange.min);
        int fearBPM = (int)(BPMdiff * fear * (1 - fatigue * 0.5f) + BPMRange.min);

        int BPM = Mathf.Max(rageBPM, fearBPM);
        BPM = Mathf.Clamp(BPM, BPMRange.min, BPMRange.max);

        return BPM;

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

    public void ToggleOnOff(bool onOff)
    {
        if(!onOff)
        {
            Wipe();
        }
        Enabled = onOff;
    }

    public void UpdateDisplay(Human human)
    {
        if(currentHuman != human)
        {
            Wipe();
            currentHuman = human;
        }
        SetBPM(CalculateBPM(human));
    }

}
