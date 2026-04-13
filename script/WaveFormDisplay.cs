using Godot;
using System;

public partial class WaveFormDisplay : Control, IDisplay
{
    public bool Enabled { get; set; } 
    private const int numBars = 50;
    [Export] private Vector2[] points = new Vector2[numBars * 2];
    [Export] private Color colour = Colors.Red;
    
    [Export] public float Speed = 5.0f;
    [Export] public float Amplitude = 1.0f;

    private float _time = 0.0f;

    public override void _Process(double delta)
    {
        _time += (float)delta * Speed;
        QueueRedraw();
    }

    public override void _Draw()
    {
        if(Enabled) 
        {
            DrawMultiline(points, colour, 2);
        }
    }

    private void UpdatePoints(Human human)
    {
        Rect2 bounds = GetRect();
        float width = bounds.Size.X;
        float height = bounds.Size.Y;
        float centerY = height / 2f;

        for (int i = 0; i < points.Length; i += 2)
        {
            float t = i / (float)(points.Length - 2);
            float xpos = t * width;

            float wave1 = Mathf.Sin((human.Stats.rage * t * Mathf.Tau) + _time);
            float wave2 = Mathf.Sin((human.Stats.fear * t * Mathf.Tau) + _time);

            float combinedWave = wave1 * wave2;
            float finalYOffset = combinedWave * (centerY * Amplitude * ((human.Stats.rage + human.Stats.fear) / 10f));

            points[i] = new Vector2(xpos, centerY + finalYOffset);
            points[i + 1] = new Vector2(xpos, centerY - finalYOffset);
        }
    }

    public void ToggleOnOff(bool onOff) 
    {
        Enabled = onOff;
    }

    public void UpdateDisplay(Human human) 
    {
        UpdatePoints(human);
        Enabled = true;
    }
}
