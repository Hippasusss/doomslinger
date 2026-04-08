using Godot;
using System;
using System.Collections.Generic;

public partial class UIManager : CanvasLayer
{
    [Export] private SectionRevealer[] UISections;
    [Export] private BlackoutController blackoutController;
    [Export] private HumanDataDisplay humanDataDisplay;

    public void CloseAllSections()
    {
        foreach(SectionRevealer section in UISections)
        {
            section.SetOpen(false);
        }
    }

    public void OnDayEnd()
    {
        CloseAllSections();
        blackoutController.OnDayEnd();
    }
}
