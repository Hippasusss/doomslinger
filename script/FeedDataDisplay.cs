using Godot;

public partial class FeedDataDisplay : Control, IDisplay
{
    [Export] private ButtonMatrix matrix;
    public bool Enabled {get; set;} = true;

    public void UpdateDisplay(Human human)
    {
        if (!Enabled || human.Feed.IsScrolling) return;
        ColorMatrix(human);
    }

    private void ColorMatrix(Human human)
    {
        var feed = human.Feed;
        var feedBlocks = feed.FeedBlocks;
        int count = feedBlocks.Count;

        int cols = matrix.Columns;

        for (int blockIdx = 0; blockIdx < matrix.Rows; blockIdx++)
        {
            int blockBeingReadIndex = feedBlocks.IndexOf(feed.GetBlockBeingRead());
            int index = Mathf.PosMod(blockBeingReadIndex + (matrix.Rows - 1 - blockIdx), count);
            Feedblock block = feedBlocks[index];

            int numStats = block.stats.mainStats.Count;
            for (int statIdx = 0; statIdx < numStats; statIdx++)
            {
                float t = (block.stats.mainStats[statIdx].GetNormalised() + 1) / 2;
                Button button = matrix.GetButton(blockIdx * cols + statIdx);
                button.SelfModulate = block.GetColour() * t;
            }
        }
    }

    public void ToggleOnOff(bool onOff)
    {
        Enabled = onOff;
        if (!onOff)
        {
            const float dim = 0.15f;
            Color grey = new(dim, dim, dim, 1);
            int count = matrix.Columns * matrix.Rows;
            for (int i = 0; i < count; i++)
                matrix.GetButton(i).SelfModulate = grey;
        }
    }
}
