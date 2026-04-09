







using Godot;
using Utils;

public partial class Human : Node2D
{
    [Signal] public delegate void HumanSelectedEventHandler(Human selected);

    [Export] private Sprite2D face;
    [Export] private Feed feed;
    [Export] private Light2D light;
    [Export] private AnimationPlayer animation;
    [Export] private Sprite2D phone;
    [Export] private Sprite2D warnignSymbol;
    private HumanStats stats = new();
    private HumanPersonalData data = new();
    private Color[] colors;
    private bool isOnline = true;

    private bool selected = false;

    public Sprite2D Face { get => face; set => face = value; }
    public HumanStats Stats { get => stats; set => stats = value; }
    public HumanPersonalData Data { get => data; set => data = value; }
    public Color[] Colors { get => colors; set => colors = value; }
    public Sprite2D Phone { get => phone; set => phone = value; }
    public Feed Feed { get => feed; set => feed = value; }
    public bool IsOnline { get => isOnline;}
    public bool Selected { get => selected; }


    public override void _Ready()
    {
        stats = new(rate: 0.7f);
        Feed.newMainFeedBlockCallBack += ReadFeedBlock;
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
                SetUserOnline(false);
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

    private void ReadFeedBlock(Feedblock block)
    {
        Stats += block.stats;
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

    public void OnClick(Node viewport, InputEvent clickEvent, long shape_idx)
    {
        if (clickEvent is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            if (mouseEvent.ButtonIndex == MouseButton.Left)
            {
                Select(!selected);
            }
        }
    }

    private void ToggleWarning(bool onOff)
    {
        warnignSymbol.Visible = onOff;
    }
}
