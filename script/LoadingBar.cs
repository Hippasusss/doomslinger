using Godot;
using System;

namespace DoomSlinger;

public partial class LoadingBar : Control
{
    [Export] private Panel loadingBarPanel;

    public void Set(float percent)
    {
        percent = Mathf.Clamp(percent,0,1);
        loadingBarPanel.AnchorRight = percent;
    }
}
