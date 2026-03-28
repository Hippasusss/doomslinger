using Godot;
using System;
using System.Collections.Generic;

public struct HumanPersonalData
{
    public string name;
    public DateOnly DOB;
    public int height;
    public string gender;
    public string nationality;
    public int UID;
    private readonly static List<int> UIDs = [];

    public HumanPersonalData(string name,
            DateOnly DOB,
            int height,
            string gender,
            string nationality)
    {
        this.name = name;
        this.DOB = DOB;
        this.height = height;
        this.gender = gender;
        this.nationality = nationality;
        do
        {
            UID = GD.RandRange(0,99999);
        }
        while(UIDs.Contains(UID));
        UIDs.Add(UID);
    }
}

