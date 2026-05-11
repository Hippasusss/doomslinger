using Godot;
using System.Collections.Generic;

public partial class GameData : Resource
{
    [Export] private Company[] companies;
    public Company[] Companies => companies;
}
