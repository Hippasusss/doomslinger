using System;
using System.Collections.Generic;
using Godot;

namespace DoomSlinger;

public readonly struct HumanPersonalData
{
    public enum Gender { Male, Female, NonBinary }
    public enum Orientation { Straight, Bi, Gay }

    public readonly string name;
    public readonly DateOnly DOB;
    public readonly int height;
    public readonly Gender gender;
    public readonly Orientation orientation;
    public readonly string nationality;
    public readonly int UID;
    private readonly static List<int> UIDs = [];

    public HumanPersonalData(string name,
            DateOnly DOB,
            int height,
            Gender gender,
            Orientation orientation,
            string nationality)
    {
        this.name = name;
        this.DOB = DOB;
        this.height = height;
        this.gender = gender;
        this.orientation = orientation;
        this.nationality = nationality;
        do
        {
            UID = GD.RandRange(0,99999);
        }
        while(UIDs.Contains(UID));
        UIDs.Add(UID);
    }
}

