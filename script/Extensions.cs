








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
        return list[GD.RandRange(0, list.Count - 1)];
    }
}

public static class SpriteExtensions
{
    public static void SetFrameFromPercent(this Sprite2D source, float percent)
    {
        percent = Mathf.Clamp(percent,0f,1f);
        if (percent < float.Epsilon)
        {
            source.Visible = false;
            source.Frame = 0;
        }
        else
        {
            source.Visible = true;
            source.Frame = (int)Mathf.Floor((float)((source.Hframes - 1) * percent));
        }
    }
}
