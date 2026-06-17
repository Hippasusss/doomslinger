using Godot;

namespace DoomSlinger;

[GlobalClass]
public partial class Nationality : Resource
{
    [Export] public string Name { get; set; }
}
