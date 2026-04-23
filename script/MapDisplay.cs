using Godot;
using System;

public partial class MapDisplay : Control, IDisplay

{
    [Export] SubViewport subView;
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
        subView.World2D = GetViewport().World2D;

        _originalSize = Size;
        _originalPosition = Position;
        _originalZIndex = ZIndex;

        mapButton.Pressed += OnClicked;
    }


    private void ToggleOpen(bool open)
    {

        _currentTween?.Kill();
        
        ZIndex = 100;

        _currentTween = GetTree().CreateTween();
        _currentTween.SetParallel(true);
        
        if(!open)
        {
            _currentTween.TweenProperty(this, "global_position", Vector2.Zero, 0.3f)
                .SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
            _currentTween.TweenProperty(this, "size", GetViewportRect().Size, 0.3f)
                .SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
        }
        else
        {

            _currentTween.TweenProperty(this, "position", _originalPosition, 0.3f)
                .SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
            _currentTween.TweenProperty(this, "size", _originalSize, 0.3f)
                .SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);


            _currentTween.Chain().TweenCallback(Callable.From(() => ZIndex = _originalZIndex));
        }
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
