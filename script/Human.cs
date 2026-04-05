







using Godot;
using System.Collections.Generic;
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

    private static List<Human> allHumans = [];
    private bool selected = false;

    public Sprite2D Face { get => face; set => face = value; }
    public HumanStats Stats { get => stats; set => stats = value; }
    public HumanPersonalData Data { get => data; set => data = value; }
    public Color[] Colors { get => colors; set => colors = value; }
    public Sprite2D Phone { get => phone; set => phone = value; }
    public Feed Feed { get => feed; set => feed = value; }
    public bool IsOnline { get => isOnline;}


    public override void _Ready()
    {
        stats = new(rate: 0.7f);
        Feed.newMainFeedBlockCallBack += ReadFeedBlock;
        allHumans.Add(this);
        HumanSelected += selected => {
            foreach(Human human in allHumans)
            {
                if(human == this) continue;
                human.DeSelect();
            };
        };
    }

    DeltaTimer warningTimerCheck = new(0.2);
    DeltaTimer exitTimer =  new(10);
    DeltaTimer statUpdateTimer =  new(0.1);
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
            if(exitTimer.Delta(delta))
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

    public void SetUserOnline(bool onOff)
    {
        isOnline = onOff;
        feed.ToggleOnOff(onOff);
        light.Enabled = onOff;
    }

    private void ReadFeedBlock(Feedblock block)
    {
        Stats += block.stats;
    }


    public void Select()
    {
        if(!selected)
        {
            animation.Play("hover");
            EmitSignal(SignalName.HumanSelected, this);
            selected = true;
        }
    }

    public void DeSelect()
    {
        if(selected)
        {
            selected = false;
            animation.PlayBackwards("hover");
        }
    }

    public void OnClick(Node viewport, InputEvent clickEvent, long shape_idx)
    {
        if (clickEvent is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            if (mouseEvent.ButtonIndex == MouseButton.Left)
            {
                Select();
            }
        }
    }

    private void ToggleWarning(bool onOff)
    {
        warnignSymbol.Visible = onOff;
    }

    // public void OnMouseEnter()
    // {
    // }
    //
    // public void OnMouseExit()
    // {
    // }
}
