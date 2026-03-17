using Godot;
using System;

public partial class PhoneLight : PointLight2D
{
    [Export] public float FlickerRange = 0.3f;
    [Export] public float FlickerSpeed = 10.0f;

    private float _targetEnergy;
    private float _currentFlickerTime;
    private float baseEnergy;

    public override void _Ready()
    {
        _targetEnergy = Energy;
        baseEnergy = Energy;
    }

    public override void _Process(double delta)
    {
        _currentFlickerTime += (float)delta;
        
        if (_currentFlickerTime >= 1.0f / FlickerSpeed)
        {
            _targetEnergy = baseEnergy + (float)GD.RandRange(-FlickerRange, FlickerRange);
            _currentFlickerTime = 0;
        }

        Energy = Mathf.Lerp(Energy, _targetEnergy, (float)delta * FlickerSpeed);
    }
}
