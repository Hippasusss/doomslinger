using Godot;
using System;

public partial class PhoneLight : PointLight2D
{
	[Export] public float BaseEnergy = 1.0f;
	[Export] public float FlickerRange = 0.3f;
	[Export] public float FlickerSpeed = 10.0f;

	private float _targetEnergy;
	private float _currentFlickerTime;

	public override void _Ready()
	{
		_targetEnergy = BaseEnergy;
	}

	public override void _Process(double delta)
	{
		_currentFlickerTime += (float)delta;
		
		// Periodically pick a new target energy to flicker towards
		if (_currentFlickerTime >= 1.0f / FlickerSpeed)
		{
			_targetEnergy = BaseEnergy + (float)GD.RandRange(-FlickerRange, FlickerRange);
			_currentFlickerTime = 0;
		}

		// Smoothly interpolate to the target energy
		Energy = Mathf.Lerp(Energy, _targetEnergy, (float)delta * FlickerSpeed);
	}
}
