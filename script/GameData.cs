using System.Collections.Generic;
using Godot;

namespace DoomSlinger;

[GlobalClass]
public partial class GameData : Resource
{
    [Export] private Company[] companies;
    [Export] private Nationality[] nationalities;
    [Export] private StringList maleFirstNames;
    [Export] private StringList femaleFirstNames;
    [Export] private StringList secondNames;

    public Company[] Companies => companies;
    public Nationality[] Nationalities => nationalities;
    public StringList MaleFirstNames => maleFirstNames;
    public StringList FemaleFirstNames => femaleFirstNames;
    public StringList SecondNames => secondNames;

    public Company GetRandomCompany()
    {
        return companies.GetRandom();
    }
}
