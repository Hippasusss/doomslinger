using Godot;

namespace DoomSlinger;

[GlobalClass]
public partial class StringList : Resource
{
    [Export] public string[] values;
}
