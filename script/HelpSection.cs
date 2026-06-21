using Godot;

namespace DoomSlinger;

public partial class HelpSection : Panel, ISection
{
    [Export] private SectionRevealer revealer;

    public void Toggle() => revealer?.Toggle();
    public void SetOpen(bool open) => revealer?.SetOpen(open);
    public bool IsOpen => revealer?.IsOpen ?? false;
}
