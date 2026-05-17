using Godot;
using System;

namespace DoomSlinger;

[GlobalClass]
public partial class Company : Resource
{

    [Export] public Texture2D Logo { get; set;}
    [Export] public string Name { get; set;}
    [Export (PropertyHint.MultilineText)] public string Description { get; set;}
    [Export] public Sector Sectors { get; set;}
    [Export] public bool IsLocal { get; set;}
    [Export] public float MarketCap { get; set;}
    [Export (PropertyHint.Range, "0, 100, 1")] public float Age { get; set;} // low is new and down with the kids, high is old and entrenched
    [Export (PropertyHint.Range, "-1, 1, 0.01")] public float PoliticalLeaning { get; set;}
    [Export (PropertyHint.Range, "0, 1, 0.01")] public float Chaos { get; set;} // low is well run, high is poorly run
    [Export (PropertyHint.Range, "0, 1, 0.01")] public float CurrentStanding { get; set;} // low is disliked, high is liked (by public)
}

[Flags]
public enum Sector { 
    Finance = 1 << 0,
    Defence = 1 << 1,
    Hospitality = 1 << 2,
    Commerce = 1 << 3,
    Agriculture = 1 << 4,
    Automotive = 1 << 5,
    Mining = 1 << 6,
    OilGas = 1 << 7,
    GreenEnergy = 1 << 8,
    Logging = 1 << 9,
    Food = 1 << 10,
    Electronics = 1 << 11,
    Pharamceutical = 1 << 12,
    Construction = 1 << 13,
    Retail = 1 << 14,
    Transport = 1 << 15,
    Healthcare = 1 << 16,
    Education = 1 << 17,
    Legal = 1 << 18,
    Telecommunications = 1 << 19,
    Entertainment = 1 << 20,
    AI = 1 << 21,
    Government = 1 << 22
}
