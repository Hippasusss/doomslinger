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
    [Export] private Sprite2D _eyeBag;
    [Export] private Sprite2D _eyeRed;
    [Export] private RichTextLabel _eyeTrackingText;
    [Export] private AnimationPlayer _animation;

    public bool Enabled {get; set;} = true;

    private readonly (float min, float max) eyeballMoveRange = (0, 18);
    private readonly (float min, float max) moveRateRange = (0.05f, 8);
    private readonly (float min, float max) irisSizeRange = (0.4f, 2);
    private readonly (float min, float max) blinkRateRange = (0.2f, 10);

    private readonly DeltaTimer eyeMoveTimer = new(0.5, 3);
    private readonly DeltaTimer blinkTimer = new(0.2, 3);

    private (float vhsAmount,float noiseAmount) CRTReset;

    private Human currentHuman = null;
    private float irisSize = 1;
    private float blinkRate = 1;
    private float moveRate = 1;
    private float redEyeAmount = 1;
    private float eyebagAmount = 1;
    private Vector2 eyeballMovePosition = new(1,1);


    public override void _Ready()
    {
        eyeMoveTimer.ForceFinish();
        blinkTimer.ForceFinish();
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
            MoveEyeball(eyeballMovePosition);
            ScaleIris(irisSize, 0.5f);
            ChangeEyeBag(eyebagAmount);
            ChangeRedeye(redEyeAmount);
            eyeMoveTimer.SetResetRange(moveRateRange.min, moveRate);
        }

        if (blinkTimer.Delta(delta))
        {
            _animation.Play("Blink");
            blinkTimer.SetResetRange(blinkRateRange.min, blinkRate + 0.5);
        }
    }

    public void MoveEyeball(Vector2 position)
    {
        _eyeBall.Position = position;
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

    public void ChangeRedeye(float value)
    {
        _eyeRed.SetFrameFromPercent(value);
    }

    public void ChangeEyeBag(float value)
    {
        _eyeBag.SetFrameFromPercent(value);
    }


    public void ToggleOnOff(bool onOff)
    {
        if(onOff)
        {
            MoveEyeball(new(4,4));
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
        if(currentHuman != human)
        {
            CalculateEye(human);
            MoveEyeball(eyeballMovePosition);
            ScaleIris(irisSize, 0);
            ChangeEyeBag(eyebagAmount);
            ChangeRedeye(redEyeAmount);
            _eyelid.Modulate = human.Colors[0];
            _eyeColor.Modulate = human.Colors[4];
            currentHuman = human;
        }
        CalculateEye(human);
    }

    private void CalculateEye(Human human)
    {
        float engagementNorm = human.Stats.engagement.GetNormalised();

        irisSize = Mathf.Lerp(irisSizeRange.min, irisSizeRange.max, 1 - engagementNorm); 
        blinkRate = Mathf.Lerp(blinkRateRange.min, blinkRateRange.max, engagementNorm);
        moveRate = Mathf.Lerp(moveRateRange.min, moveRateRange.max, 1 - engagementNorm);
        redEyeAmount = human.Stats.fatigue.GetNormalised();
        eyebagAmount = human.Stats.longTermFatigue.GetNormalised();

        const float minMove = 1f;
        float currentRadius = eyeballMoveRange.max * (1.0f - engagementNorm) + minMove;

        float yOffset = eyeballMoveRange.max/2 * engagementNorm;

        float angle = GD.Randf() * Mathf.Tau;
        float distance = Mathf.Sqrt(GD.Randf()) * currentRadius;

        Vector2 randomPoint = Vector2.FromAngle(angle) * distance;
        eyeballMovePosition = new Vector2(0, yOffset) + randomPoint;

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
