using Godot;

namespace DoomSlinger;

public partial class Company : Resource
{
    [Export] Texture2D logo;
    [Export] private string Name;
    [Export] private string Description;
    [Export] private string Sector;
}
