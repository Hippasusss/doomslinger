using Godot;
using System;
using System.Collections.Generic;

public partial class BgGenerator : Node2D
{
    [Export] public float BaseParallaxAmount = 0.5f;
    [Export] public float VerticalParallaxFactor = 0.001f;

    private Camera2D _camera;
    private List<Node2D> _layers = new List<Node2D>();
    private List<Vector2> _basePositions = new List<Vector2>();
    private List<float> _parallaxWeights = new List<float>();

    public override void _Ready()
    {
        _camera = GetViewport().GetCamera2D();

        // Store initial positions and calculate weights based on Y position
        foreach (Node child in GetChildren())
        {
            if (child is Node2D node2D)
            {
                _layers.Add(node2D);
                _basePositions.Add(node2D.Position);
                
                // Using Y position to influence parallax weight
                // Higher Y (lower on screen) will have different parallax than lower Y
                float weight = BaseParallaxAmount + (node2D.Position.Y * VerticalParallaxFactor);
                _parallaxWeights.Add(weight);
            }
        }
    }

    public override void _Process(double delta)
    {
        if (_camera == null)
        {
            _camera = GetViewport().GetCamera2D();
            if (_camera == null) return;
        }

        Vector2 cameraPos = _camera.GlobalPosition;
        
        for (int i = 0; i < _layers.Count; i++)
        {
            // Apply unique parallax for each layer based on its weight
            _layers[i].Position = _basePositions[i] + (cameraPos * _parallaxWeights[i]);
        }
    }
}
