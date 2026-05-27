using Godot;
using System;

namespace DoomSlinger;

public partial class Human : Node2D
{
    [Signal] public delegate void HumanSelectedEventHandler(Human selected);

    [Export] private Sprite2D face;
    [Export] private Feed feed;
    [Export] private Light2D light;
    [Export] private AnimationPlayer animation;
    [Export] private Sprite2D phone;
    [Export] private Sprite2D warnignSymbol;
    [Export] private Sprite2D movingSymbol;
    [Export] private Sprite2D shockSymbol;
    [Export] private Button SelectionButton;

    private HumanStats stats = new();
    private HumanPersonalData data = new();
    private bool isOnline = true;
    private bool isMoving = false;
    private bool selected = false;

    public HumanStats Stats { get => stats; }
    public HumanPersonalData Data { get => data; set => data = value; }

    public Color ColorFace { get; set; }
    public Color ColorHair { get; set; }
    public Color ColorClothes { get; set; }
    public Color ColorTrim { get; set; }
    public Color ColorEyes { get; set; }
    public Color ColorPhone { get; set; }

    public Sprite2D Face { get => face; }
    public Sprite2D Phone { get => phone; }
    public Feed Feed { get => feed; }

    public bool IsOnline { get => isOnline;}
    public bool IsMoving { get => isMoving; }
    public bool Selected { get => selected; }

    public int BPM
    {
        get
        {
            (int min, int max) = (35, 199);
            int BPMdiff = max- min;

            float seratonin = Stats.Seratonin.GetNormalised();
            float cortisol = Stats.Cortisol.GetNormalised();
            float melatonin = Stats.Melatonin.GetNormalised();

            int rageBPM = (int)(BPMdiff * seratonin * (1 - melatonin * 0.5f) + min);
            int fearBPM = (int)(BPMdiff * cortisol * (1 - melatonin * 0.5f) + min);

            int final = Mathf.Max(rageBPM, fearBPM);
            return Mathf.Clamp(final, min, max);
        }
    }

    public override void _Ready()
    {
        stats = new(rate: 0.7f);
        Feed.newMainFeedBlockCallBack += ReadFeedBlock;
        SelectionButton.Pressed += () => {Select(!selected);};
    }

    private readonly DeltaTimer warningTimerCheck = new(0.2);
    private readonly DeltaTimer offlineReturnTimer =  new(10);
    private readonly DeltaTimer statUpdateTimer =  new(0.1);
    public override void _Process(double delta)
    {
        const float warningLevel = 0.79f;
        const float logOffLevel = 0.99f;
        if(warningTimerCheck.Delta(delta))
        {
            if(stats.AreAnyOver(warningLevel))
            {
                ToggleWarning(true);
            }
            else
            {
                ToggleWarning(false);
            }
            if(stats.AreAnyOver(logOffLevel))
            {
                // SetUserOnline(false);
            }
        }
        if(!isOnline)
        {
            if(offlineReturnTimer.Delta(delta))
            {
                SetUserOnline(true);
            }
            stats.CoolDown(delta);
        }
        if(statUpdateTimer.Delta(delta))
        {
            stats.UpdateAll(delta);
        }
    }

    public void SetUserOnline(bool setOnline, bool withCooldown = true)
    {
        isOnline = setOnline;
        feed.ToggleOnOff(setOnline);
        light.Enabled = setOnline;
        if(!setOnline)
        {
            Select(false);
            SetMoving(false);
            warningTimerCheck.Stop();
            statUpdateTimer.Stop();
            if(withCooldown)
            {
                offlineReturnTimer.Reset();
                offlineReturnTimer.Start();
            }
            else
            {
                offlineReturnTimer.Reset();
                offlineReturnTimer.Stop();
            }
        }
        else
        {
            warningTimerCheck.Start();
            statUpdateTimer.Start();
            offlineReturnTimer.Reset();
            offlineReturnTimer.Stop();
        }

    }

    public void SetMoving(bool setMoving)
    {
        isMoving = setMoving;
        ToggleMoving(setMoving);
    }

    private void ReadFeedBlock(Feedblock block)
    {
        if(block == null)
        {
            ToggleShock(true);
            return;
        }
        else
        {
            ToggleShock(false);
            float length = block.BlockData.Length;
            float politicalLeaning = block.BlockData.PoliticalLeaning;
        }
    }

    public void Select(bool setSelected, bool emit = true)
    {
        if(setSelected && !selected && isOnline)
        {
            animation.Play("hover");
            selected = true;
            if(emit) EmitSignal(SignalName.HumanSelected, this);
        }
        else if (!setSelected && selected)
        {
            animation.PlayBackwards("hover");
            selected = false;
            if(emit) EmitSignal(SignalName.HumanSelected, this);
        }
    }

    private void ToggleWarning(bool onOff)
    {
        warnignSymbol.Visible = onOff;
    }

    private void ToggleMoving(bool onOff)
    {
        movingSymbol.Visible = onOff;
    }

    private void ToggleShock(bool onOff)
    {
        shockSymbol.Visible = onOff;
    }
}
