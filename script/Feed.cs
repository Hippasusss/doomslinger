








using Godot;
using System;
using System.Collections.Generic;
using Utils;

public partial class Feed : CanvasGroup 
{
    [Export] private PackedScene feedBlockSN;
    [Export] private ColorPalette feedBlockPalette;
    [Export] private Light2D screenLight;
    [Export] private Sprite2D generator;

    public Action<Feedblock> newMainFeedBlockCallBack;
    public List<Feedblock> FeedBlocks => feedBlocks;
    public bool IsScrolling { get; private set; }

    private const int numBlocks = 7;
    private const int blockSpacing = 52;
    private const float turnOffRate = 0.5f;
    private readonly List<Feedblock> feedBlocks = [];
    private bool enabled = true;
    private int currentBlockIndex = 0;

    private readonly DeltaTimer swipeTimer = new(3, 14);

    public override async void _Ready()
    {
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        Vector2 globalPos = GlobalPosition;
        Vector2 screenSize = GetViewportRect().Size;

        generator.Position = new(Position.X, -globalPos.Y);

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
            Feedblock newfeedblock = feedBlockSN.Instantiate<Feedblock>();
            AddChild(newfeedblock);
            FeedBlocks.Add(newfeedblock);
            ResetBlock(newfeedblock);
        }
        screenLight.Color = GetBlockBeingRead().GetColour();
    }

    public override void _Process(double delta) 
    {
        if(swipeTimer.Delta(delta) && enabled)
        {
            AdvanceFeed();
        }
    }

    private void AdvanceFeed()
    {
        IsScrolling = true;
        Tween tween = CreateTween().SetParallel(true);
        float duration = 0.4f;
        int nextBlockIndex =  (currentBlockIndex + 1) % numBlocks;

        foreach (Feedblock block in FeedBlocks)
        {
            tween.TweenProperty(block, "position:y", block.Position.Y + blockSpacing, duration)
                .SetTrans(Tween.TransitionType.Quart)
                .SetEase(Tween.EaseType.Out);
        }
        tween.TweenProperty(screenLight, "color", feedBlocks[nextBlockIndex].GetColour(), duration)
            .SetTrans(Tween.TransitionType.Quart)
            .SetEase(Tween.EaseType.Out);

        tween.Chain().TweenCallback(Callable.From(() =>
                    {
                    ResetBlock(GetBlockBeingRead());
                    currentBlockIndex = nextBlockIndex;
                    newMainFeedBlockCallBack?.Invoke(GetBlockBeingRead());
                    IsScrolling = false;
                    }));
    }

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

    public Feedblock GetBlockBeingRead() => feedBlocks[currentBlockIndex];

    private void ResetBlock(Feedblock block)
    {
        block.Position = new Vector2(block.Position.X, (FeedBlocks.Count - 1) * -blockSpacing);
        block.SetColour(feedBlockPalette.Colors.GetRandom());
        block.blockData.Randomize();
    }
}
