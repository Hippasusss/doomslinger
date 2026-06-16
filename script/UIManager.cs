using System;
using System.Collections.Generic;
using Godot;

namespace DoomSlinger;

public partial class UIManager : CanvasLayer
{
    [Export] private Node[] UISections;
    [Export] private BlackoutController blackoutController;

    public void CloseAllSections()
    {
        foreach(var s in UISections)
        {
            if (s is ISection section)
                section.SetOpen(false);
            else if (s is SectionRevealer revealer)
                revealer.SetOpen(false);
        }
    }

    public void OnDayEnd()
    {
        CloseAllSections();
        blackoutController.OnDayEnd();
    }
}
