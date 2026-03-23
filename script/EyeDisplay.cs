using Godot;
using System;
using Utils;

public partial class EyeDisplay : Node2D, IDisplay
{
    [Export] private Node2D _eyeBall;
    [Export] private Sprite2D _clipCRT;
    [Export] private Sprite2D _iris;
    [Export] private Sprite2D _eyelid;
    [Export] private Sprite2D _eyeColor;
    [Export] private RichTextLabel _eyeTrackingText;
    [Export] private AnimationPlayer _animation;

    public bool Enabled {get; set;} = true;

    private const float eyeballMoveRange = 8;
    private readonly (float min, float max) irisSizeRange;

    private readonly DeltaTimer eyeMoveTimer = new(0.5, 3);
    private readonly DeltaTimer blinkTimer = new(0.2, 3);

    private (float vhsAmount,float noiseAmount) CRTReset;

    public override void _Ready()
    {
        if (_clipCRT.Material is ShaderMaterial sm)
        {
            CRTReset.vhsAmount = (float)sm.GetShaderParameter("vhs_intensity");
            CRTReset.noiseAmount = (float)sm.GetShaderParameter("noise_intensity");
        }
    }

    public override void _Process(double delta)
    {
        if(!Enabled) return;
        if (eyeMoveTimer.Delta(delta))
        {
            MoveEyeball(eyeballMoveRange);

            float randomScale = (float)GD.RandRange(0.5, 1.5);
            ScaleIris(randomScale, 0.5f);
        }

        if (blinkTimer.Delta(delta))
        {
            _animation.Play("Blink");
        }
    }

    public void MoveEyeball(float range)
    {
        Vector2 randomPos = new((float)GD.RandRange(-range, range), (float)GD.RandRange(-range, range));
        _eyeBall.Position = randomPos;
    }

    private Tween irisTween;
    public void ScaleIris(float targetScale, float duration = 0.2f)
    {
        if (_iris == null) return;
        if(duration <= float.Epsilon)
        {
            irisTween?.Kill();
            _iris.Scale = new(targetScale, targetScale);
        }
        else
        {
            irisTween = CreateTween();
            irisTween.TweenProperty(_iris, "scale", new Vector2(targetScale, targetScale), duration)
                .SetTrans(Tween.TransitionType.Quart)
                .SetEase(Tween.EaseType.Out);
        }
    }


    public void ToggleOnOff(bool onOff)
    {
        if(onOff)
        {
            MoveEyeball(4);
            ScaleIris((float)GD.RandRange(0.5, 1.5), 0);
            SetCRTAmount(CRTReset.vhsAmount, CRTReset.noiseAmount, false);
        }
        else
        {
            SetCRTAmount(10,1, true);
        }
        _eyeTrackingText.Text = onOff ? "eye tracking: [color=green]enabled[/color]" : "eye tracking: [color=red]disabled[/color]";
        Enabled = onOff;
    }

    public void UpdateDisplay(Human human)
    {
        _eyelid.Modulate = human.Colors[0];
        _eyeColor.Modulate = human.Colors[4];
    }

    private void CalculateEye(Human human)
    {
        float eyeMovementRate = 0;
        float eyeMovementRange = 0;
        float blinkRate = 0;
        float irisSize = 0;
    }

    private void SetCRTAmount(float vhs, float noise, bool grey)
    {
        if(_clipCRT.Material is ShaderMaterial sm)
        {
            sm.SetShaderParameter("vhs_intensity", vhs);
            sm.SetShaderParameter("noise_intensity", noise);
            sm.SetShaderParameter("greyscale", grey);
        }
    }
}
