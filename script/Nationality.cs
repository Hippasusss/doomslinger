using Godot;

namespace DoomSlinger;

[GlobalClass]
public partial class Nationality : Resource
{
    [Export] public string Name { get; set; }
    [Export] public Color IDColor { get; set; }
    [Export] public Texture2D Flag { get; set; }
}
