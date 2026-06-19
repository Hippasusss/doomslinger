
using Godot;

public static class Helpers
{
    public static string FormatLargeDollarAmount(float value)
    {
        (float threshold, string suffix)[] tiers =
        {
            (1_000_000_000_000f, "T"),
            (1_000_000_000f, "B"),
            (1_000_000f, "M"),
            (1_000f, "K"),
        };

        foreach (var (threshold, suffix) in tiers)
        {
            if (value >= threshold)
            {
                float divided = value / threshold;
                string formatted = divided % 1 == 0
                    ? $"${divided:0}{suffix}"
                    : $"${divided:0.#}{suffix}";
                return formatted;
            }
        }

        return $"${value:0}";
    }
}

