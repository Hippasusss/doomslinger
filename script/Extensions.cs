








using Godot;
using System.Collections.Generic;
using System.Linq;

public static class EnumerableExtensions
{
	public static T GetRandom<T>(this IEnumerable<T> source)
	{
		if (source == null) return default;
		var list = source.ToList();
		if (list.Count == 0) return default;
		return list[(int)GD.RandRange(0, list.Count - 1)];
	}
}
