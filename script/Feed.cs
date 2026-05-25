using System;
using System.Collections.Generic;
using Godot;

namespace DoomSlinger;

public partial class Feed : CanvasGroup
{
    [Export] private PackedScene feedBlockSN;
    [Export] private ColorPalette feedBlockPalette;
    [Export] private Light2D screenLight;
    [Export] private Sprite2D generator;

    public Action<Feedblock> newMainFeedBlockCallBack;
    public Func<BlockData> OnRequestBlockData;
    public List<Feedblock> FeedBlocks => feedBlocks;
    public bool IsScrolling { get; private set; }

    private const int numBlocks = 7;
    private const int blockSpacing = 52;
    private const float turnOffRate = 0.5f;
    private const float cascadeTop = (numBlocks - 1) * -blockSpacing;

    private readonly List<Feedblock> feedBlocks = [];
    private readonly List<Feedblock> active = [];
    private readonly Stack<Feedblock> pool = new();
    private bool enabled = true;

    private readonly DeltaTimer swipeTimer = new(3, 14);

    public override async void _Ready()
    {
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        Vector2 globalPos = GlobalPosition;
        Vector2 screenSize = GetViewportRect().Size;

        generator.Position = new(Position.X, -globalPos.Y - 10f);

        if(Material is ShaderMaterial mat)
        {
            const float fadeDistance= 0.1f;
            const float fadeOffset = 0.05f;
            float screenUV = Mathf.Clamp(globalPos.Y / screenSize.Y, 0, 1);
            mat.SetShaderParameter("fade_start", screenUV - fadeDistance + fadeOffset);
            mat.SetShaderParameter("fade_end", screenUV + fadeOffset);
        }

        for(int i = 0; i < numBlocks; i++)
        {
            Feedblock fb = feedBlockSN.Instantiate<Feedblock>();
            AddChild(fb);
            feedBlocks.Add(fb);
            fb.Reset(new());
            fb.Position = new Vector2(fb.Position.X, i * -blockSpacing);
            active.Add(fb);
        }
        screenLight.Color = Colors.White;
    }

    public override void _Process(double delta)
    {
        if(swipeTimer.Delta(delta) && enabled)
            AdvanceFeed();
    }

    private void AdvanceFeed()
    {
        IsScrolling = true;
        Tween phase1 = CreateTween().SetParallel(true);
        float duration = 0.4f;

        if (active.Count > 0)
        {
            for (int i = 0; i < active.Count; i++)
                phase1.TweenProperty(active[i], "position:y", active[i].Position.Y + blockSpacing, duration)
                    .SetTrans(Tween.TransitionType.Quart).SetEase(Tween.EaseType.Out);

            if (active.Count > 1)
                phase1.TweenProperty(screenLight, "color", active[1].GetColour(), duration)
                    .SetTrans(Tween.TransitionType.Quart).SetEase(Tween.EaseType.Out);
        }

        phase1.Chain().TweenCallback(Callable.From(() =>
        {
            if (active.Count > 0)
            {
                active[0].Visible = false;
                pool.Push(active[0]);
                active.RemoveAt(0);
            }

            while (active.Count < numBlocks)
            {
                BlockData bid = OnRequestBlockData?.Invoke();
                if (bid == null || pool.Count == 0) break;
                Feedblock block = pool.Pop();
                block.Reset(bid);
                block.Position = new Vector2(block.Position.X, cascadeTop);
                active.Add(block);
            }

            if (active.Count > 0)
            {
                Tween phase2 = CreateTween().SetParallel(true);
                for (int i = 0; i < active.Count; i++)
                {
                    if (active[i].Position.Y == i * -blockSpacing) continue;
                    phase2.TweenProperty(active[i], "position:y", i * -blockSpacing, 0.3f)
                        .SetTrans(Tween.TransitionType.Quart).SetEase(Tween.EaseType.Out);
                }
                phase2.Chain().TweenCallback(Callable.From(() =>
                {
                    newMainFeedBlockCallBack?.Invoke(active[0]);
                    IsScrolling = false;
                }));
            }
            else
            {
                newMainFeedBlockCallBack?.Invoke(null);
                IsScrolling = false;
            }
        }));
    }

    public Feedblock GetBlockBeingRead() => active.Count > 0 ? active[0] : null;

    public void ToggleOnOff(bool onOff)
    {
        enabled = onOff;
        if(onOff)
        {
            Tween tween = CreateTween();
            tween.TweenProperty(this, "modulate", Colors.White, turnOffRate)
                .SetTrans(Tween.TransitionType.Quart)
                .SetEase(Tween.EaseType.Out);
        }
        else
        {
            const float darkness = 0.2f;
            Color dark = new(darkness,darkness,darkness,1);
            Tween tween = CreateTween();
            tween.TweenProperty(this, "modulate", dark, turnOffRate)
                .SetTrans(Tween.TransitionType.Quart)
                .SetEase(Tween.EaseType.Out);
        }
    }
}
