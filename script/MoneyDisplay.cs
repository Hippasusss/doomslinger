using System.Collections.Generic;
using Godot;

namespace DoomSlinger;

public partial class MoneyDisplay : Control
{
	[Export] private PackedScene amountScene;
	[Export] private int initialPoolSize = 0;

	private readonly List<MoneyAmount> pool = [];

	public override void _Ready()
	{
		foreach (Node child in GetChildren())
		{
			if (child is MoneyAmount amount) pool.Add(amount);
		}

		while (pool.Count < initialPoolSize)
		{
			SpawnIntoPool();
		}
	}

	public void DisplayAmount(float amount)
	{
		MoneyAmount target = null;
		foreach (MoneyAmount pooled in pool)
		{
			if (pooled.IsIdle)
			{
				target = pooled;
				break;
			}
		}

		target ??= SpawnIntoPool();
		target?.Display(amount);
	}

	private MoneyAmount SpawnIntoPool()
	{
		if (amountScene == null)
		{
			GD.PushError($"{Name} has no amount scene assigned.");
			return null;
		}

		MoneyAmount amount = amountScene.Instantiate<MoneyAmount>();
		AddChild(amount);
		pool.Add(amount);
		return amount;
	}
}
