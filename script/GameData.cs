using System.Collections.Generic;
using Godot;

namespace DoomSlinger;

[GlobalClass]
public partial class GameData : Resource
{
    [Export] private Company[] companies;
    [Export] private string[] nationalities;
    [Export] private string[] maleFirstNames;
    [Export] private string[] femaleFirstNames;
    [Export] private string[] secondNames;
    public Company[] Companies => companies;

    public Company GetRandomCompany()
    {
        return companies.GetRandom();
    }
}
