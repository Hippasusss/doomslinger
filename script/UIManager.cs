using Godot;
using System;
using System.Collections.Generic;

public partial class UIManager : CanvasLayer
{
    [Export] private SectionRevealer[] UISections;

    public void CloseAllSections()
    {
        foreach(SectionRevealer section in UISections)
        {
            section.Close();
        }
    }
}
