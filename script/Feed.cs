








using Godot;
using System;
using System.Text;
using System.Collections.Generic;
using Utils;

public partial class Feed : CanvasGroup 
{
    [Export] private PackedScene feedBlockSN;
    [Export] private ColorPalette feedBlockPalette;
    [Export] private Light2D screenLight;
    [Export] private Sprite2D generator;
    [Export] private Sprite2D face;

    public Action<Feedblock> newMainFeedBlockCallBack;
    public List<Feedblock> FeedBlocks => feedBlocks;

    private const int numBlocks = 9;
    private const int blockSpacing = 52;
    private const float fadeDistance= 0.1f;
    private const float fadeOffset = 0.05f;
    private readonly List<Feedblock> feedBlocks = [];
    private bool enabled = true;

    public override async void _Ready()
    {
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        Vector2 globalPos = GlobalPosition;
        Vector2 screenSize = GetViewportRect().Size;

        generator.Position = new(Position.X, -globalPos.Y);

        if(Material is ShaderMaterial mat)
        {
            float screenUV = Mathf.Clamp(globalPos.Y / screenSize.Y, 0, 1);
            mat.SetShaderParameter("fade_start", screenUV - fadeDistance + fadeOffset);
            mat.SetShaderParameter("fade_end", screenUV + fadeOffset);
        }


        for(int i = 0; i < numBlocks; i++)
        {
            Feedblock newfeedblock = feedBlockSN.Instantiate<Feedblock>();
            AddChild(newfeedblock);
            FeedBlocks.Add(newfeedblock);
            ResetBlock(newfeedblock);
            if (CheckBlockDelta(newfeedblock, 0))
            {
                screenLight.Color = newfeedblock.GetColour();
            }
        }
    }

    private readonly DeltaTimer swipeTimer = new(3, 14);
    public override void _Process(double delta) 
    {
        if(swipeTimer.Delta(delta) && enabled)
        {
            AdvanceFeed();
        }
    }

    private void AdvanceFeed()
    {
        Tween tween = CreateTween().SetParallel(true);
        float duration = 0.4f;

        foreach (Feedblock block in FeedBlocks)
        {
            tween.TweenProperty(block, "position:y", block.Position.Y + blockSpacing, duration)
                .SetTrans(Tween.TransitionType.Quart)
                .SetEase(Tween.EaseType.Out);

            if (CheckBlockDelta(block, -1))
            {
                tween.TweenProperty(screenLight, "color", block.GetColour(), duration)
                    .SetTrans(Tween.TransitionType.Quart)
                    .SetEase(Tween.EaseType.Out);
            }


        }

        tween.Chain().TweenCallback(Callable.From(() =>
        {
            foreach (Feedblock block in FeedBlocks)
            {
                if (block.Position.Y > 100)
                {
                    ResetBlock(block);
                }
                if(CheckBlockDelta(block, 0))
                {
                    newMainFeedBlockCallBack?.Invoke(block);
                }
            }
        }));
    }

    public void ToggleOnOff(bool onOff)
    {
        enabled = onOff;
        if(onOff)
        {
            Tween tween = CreateTween();
            tween.TweenProperty(this, "modulate", Colors.White, 0.4f)
                .SetTrans(Tween.TransitionType.Quart)
                .SetEase(Tween.EaseType.Out);
        }
        else
        {
            const float darkness = 0.2f;
            Color dark = new(darkness,darkness,darkness,1);
            Tween tween = CreateTween();
            tween.TweenProperty(this, "modulate", dark, 0.4f)
                .SetTrans(Tween.TransitionType.Quart)
                .SetEase(Tween.EaseType.Out);
            // Modulate = dark;
        }
    }

    public Feedblock GetBlockBeingRead()
    {
        foreach(Feedblock block in feedBlocks)
        {
            if(CheckBlockDelta(block, 0))
            {
                return block;
            }
        }
        return null;
    }

    private static bool CheckBlockDelta(Feedblock block, int delta)
    {
        if (Mathf.Abs(block.Position.Y + (blockSpacing * -delta)) < 1.0f) return true;
        return false;
    }


    private void ResetBlock(Feedblock block)
    {
        block.Position = new Vector2(block.Position.X, (FeedBlocks.Count - 2) * -blockSpacing);
        block.SetColour(feedBlockPalette.Colors.GetRandom());
        block.stats.RandomizeStats();
    }
}
