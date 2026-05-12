using System.Collections.Generic;
using Godot;

namespace DoomSlinger;

public partial class GameData : Resource
{
    [Export] private Company[] companies;
    public Company[] Companies => companies;
}
