using Godot;

namespace DoomSlinger;

[GlobalClass]
public partial class ContentCategory : Resource
{
    [Export] public string Name { get; set; }
    [Export] public Color Color { get; set; }
}
