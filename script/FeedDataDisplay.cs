using Godot;
using System;
using System.Text;

public partial class FeedDataDisplay : Control, IDisplay
{
    [Export] private RichTextLabel label;

    public bool Enabled {get; set;} = true;

    public void UpdateDisplay(Human human)
    {
        const int spacing = 3;
        if(!Enabled) return;
        label.Text = GetBlockData(human.Feed);
        label.Text += GetStatsString(human.Stats);
        label.Newline();
        label.AppendText($"{"f", spacing}{ "r", spacing}{ "m", spacing}{ "h", spacing}{"f", spacing}");
    }

    public void ToggleOnOff(bool onOff)
    {
        if(!onOff)
        {
            label.Text = "no data";
        }
        Enabled = onOff;
    }

    public static string GetStatsString(HumanStats humanStats)
    {
        const int spacing = -2;
        return $"{FormatStat(humanStats.fear.Value), spacing} {FormatStat(humanStats.rage.Value), spacing} {FormatStat(humanStats.dopamine.Value), spacing} {FormatStat(humanStats.hunger.Value), spacing} {FormatStat(humanStats.fatigue.Value), spacing}";
    }

    private static string FormatStat(float value) 
    {
        string color;
        char arrow;
        if(value > 0)
        {
            color = "green";
            arrow = '↑';
        }
        else if(value < 0)
        {
            color = "red";
            arrow = '↓';
        }
        else
        {
            color = "";
            arrow = '~';
        }
        const string formatting = "F0";
        return $"[color={color}]{Math.Abs(value).ToString(formatting)}{arrow}[/color]";
    }

    public static string GetBlockData(Feed feed)
    {
        var feedBlocks = feed.FeedBlocks;
        if (feedBlocks.Count == 0) return string.Empty;

        StringBuilder text = new();
        int count = feedBlocks.Count;

        for (int i = 0; i < count; i++)
        {
            int currentIndex = (feedBlocks.IndexOf(feed.GetBlockBeingRead()) - i + count) % count;

            Feedblock block = feedBlocks[currentIndex];

            text.AppendLine($"| {GetStatsString(block.stats)} |");
        }

        return text.ToString();
    }

}
