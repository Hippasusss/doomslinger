using Godot;

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
    [Export] private Button SelectionButton;

    private HumanStats stats = new();
    private HumanPersonalData data = new();
    private Color[] colors;
    private bool isOnline = true;
    private bool isMoving = false;
    private bool selected = false;

    public HumanStats Stats { get => stats; }
    public HumanPersonalData Data { get => data; set => data = value; }
    public Color[] Colors { get => colors; set => colors = value; } 

    public Sprite2D Face { get => face; }
    public Sprite2D Phone { get => phone; }
    public Feed Feed { get => feed; }

    public int BPM {get ; set;}

    public bool IsOnline { get => isOnline;}
    public bool IsMoving { get => isMoving; }
    public bool Selected { get => selected; }


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
            CalculateBPM();
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
        // Stats.AddOther(block.stats);
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

    private readonly (int min, int max) BPMRange = (35, 199);
    private void CalculateBPM()
    {
        int BPMdiff = BPMRange.max- BPMRange.min;

        float rage = Stats.rage.GetNormalised();
        float fear = Stats.fear.GetNormalised();
        float fatigue = Stats.fatigue.GetNormalised();

        int rageBPM = (int)(BPMdiff * rage * (1 - fatigue * 0.5f) + BPMRange.min);
        int fearBPM = (int)(BPMdiff * fear * (1 - fatigue * 0.5f) + BPMRange.min);

        BPM = Mathf.Max(rageBPM, fearBPM);
        BPM = Mathf.Clamp(BPM, BPMRange.min, BPMRange.max);

    }

    private void ToggleWarning(bool onOff)
    {
        warnignSymbol.Visible = onOff;
    }

    private void ToggleMoving(bool onOff)
    {
        movingSymbol.Visible = onOff;
    }
}
