using System;
using Godot;

namespace DoomSlinger;

public partial class PhoneLight : PointLight2D
{
    [Export] public float FlickerRange = 0.3f;
    [Export] public float FlickerSpeed = 10.0f;

    [Export] public float EnergyOffset = 0f;
    private float baseEnergy;
    private float _targetEnergy;

    public override void _Ready()
    {
        flickerTimer = new(1/FlickerSpeed);
        _targetEnergy = Energy;
        baseEnergy = Energy;
    }

    public DeltaTimer flickerTimer = new(1);
    public override void _Process(double delta)
    {

        if (flickerTimer.Delta(delta))
        {
            _targetEnergy = baseEnergy + (float)GD.RandRange(-FlickerRange, FlickerRange);
        }

        Energy = Mathf.Lerp(Energy, _targetEnergy, (float)delta * FlickerSpeed) + EnergyOffset;
    }
}
