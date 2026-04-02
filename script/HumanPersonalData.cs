using Godot;
using System;
using System.Collections.Generic;

public struct HumanPersonalData
{
    public readonly string name;
    public readonly DateOnly DOB;
    public readonly int height;
    public readonly string gender;
    public readonly string nationality;
    public readonly int UID;
    public int meaningfullConnections;
    private readonly static List<int> UIDs = [];

    public HumanPersonalData(string name,
            DateOnly DOB,
            int height,
            string gender,
            string nationality,
            int meaningfullConnections)
    {
        this.name = name;
        this.DOB = DOB;
        this.height = height;
        this.gender = gender;
        this.nationality = nationality;
        this.meaningfullConnections = meaningfullConnections;
        do
        {
            UID = GD.RandRange(0,99999);
        }
        while(UIDs.Contains(UID));
        UIDs.Add(UID);
    }
}

