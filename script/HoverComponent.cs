using Godot;

public partial class HoverComponent : Sprite2D
{
    [Export] public float Radius = 5.0f;
    [Export] public float Speed = 2.0f;

    private Vector2 _startPos;
    private float _time;

    public override void _Ready()
    {
        _startPos = Position;
        _time = GD.Randf() * 100.0f; 
    }

    public override void _Process(double delta)
    {
        _time += (float)delta * Speed;
        
        float x = Mathf.Sin(_time) * Radius;
        float y = Mathf.Cos(_time * 0.7f) * Radius;

        Position = _startPos + new Vector2(x, y);
    }
}
