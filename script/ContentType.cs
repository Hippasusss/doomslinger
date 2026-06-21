using Godot;

namespace DoomSlinger;

[GlobalClass]
public partial class ContentType : Resource
{
    [Export] public string Name { get; set; }
    [Export] public ContentCategory Category { get; set; }
}
