using Godot;
using System;
using System.Text;

public partial class FeedDataDisplay : Control, IDisplay
{
    [Export] private ButtonMatrix matrix;

    public bool Enabled {get; set;} = true;

    public void UpdateDisplay(Human human)
    {
        if (!Enabled) return;
        ColorMatrix(human);
    }

    private void ColorMatrix(Human human)
    {
        var feed = human.Feed;
        var feedBlocks = feed.FeedBlocks;
        int count = feedBlocks.Count;
        if (count == 0) return;

        int cols = matrix.Columns;

        for (int blockIdx = 0; blockIdx < matrix.Rows; blockIdx++)
        {
            int index = (feedBlocks.IndexOf(feed.GetBlockBeingRead()) - blockIdx + count) % count;
            Feedblock block = feedBlocks[index];

            for (int statIdx = 0; statIdx < 5; statIdx++)
            {
                float t = block.stats.mainStats[statIdx].GetNormalised();
                Button button = matrix.GetButton(blockIdx * cols + statIdx);
                button.SelfModulate = new Color(t, t, t, 1);
            }
        }
    }

    public void ToggleOnOff(bool onOff)
    {
        matrix.Visible = onOff;
        Enabled = onOff;
    }
}
