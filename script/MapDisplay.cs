using Godot;
using System;

public partial class MapDisplay : Control, IDisplay

{
    [Export] private SubViewport mapViewport;
    [Export] private TextureRect mapPreview;
    [Export] Button mapButton;


    private Vector2 _originalSize;
    private Vector2 _originalPosition;
    private int _originalZIndex;
    private Tween _currentTween;
    private bool currentlyOpen = false;

    public bool Enabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public void ToggleOnOff(bool onOff)
    {
        throw new NotImplementedException();
    }

    public void UpdateDisplay(Human human)
    {
        throw new NotImplementedException();
    }


    public override void _Ready()
    {
        if (mapViewport != null && mapPreview != null)
        {
            mapPreview.Texture = mapViewport.GetTexture();
        }

        _originalSize = Size;
        _originalPosition = Position;
        _originalZIndex = ZIndex;

        mapButton.Pressed += OnClicked;
    }


    private void ToggleOpen(bool open)
    {
        _currentTween?.Kill();
        ZIndex = 100;

        Vector2 targetPos = !open ? Vector2.Zero : _originalPosition;
        Vector2 targetSize = !open ? GetViewportRect().Size : _originalSize;

        _currentTween = GetTree().CreateTween().SetParallel(true);

        _currentTween.TweenProperty(this, open ? "position" : "global_position", targetPos, 0.3f)
            .SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);

        _currentTween.TweenProperty(this, "size", targetSize, 0.3f)
            .SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);

        if (open)
            _currentTween.Chain().TweenCallback(Callable.From(() => ZIndex = _originalZIndex));

        currentlyOpen = !open;
    }

    private void OnClicked()
    {
        ToggleOpen(currentlyOpen);
    }

    private void OnExit()
    {
        ToggleOpen(false);
    }

}
