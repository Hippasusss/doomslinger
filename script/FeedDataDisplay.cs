using Godot;
using System;
using System.Text;

public partial class FeedDataDisplay : Control, IDisplay
{
    [Export] private RichTextLabel label;

    public bool Enabled {get; set;} = true;

    public void UpdateDisplay(Human human)
    {
        if(!Enabled) return;
        label.Text = human.Feed.GetBlockData();
    }

    public void ToggleOnOff(bool onOff)
    {
        if(!onOff)
        {
            label.Text = "no data";
        }
        Enabled = onOff;
    }

}
