using Godot;
using System;

namespace DoomSlinger;

public partial class BidDataDisplay : Panel
{
    [Export] public TextureRect CompanyLogo { get; set;}
    [Export] public RichTextLabel CompanyName { get; set;}
    [Export] public RichTextLabel CompanyInfo { get; set;}
    [Export] public RichTextLabel CompanySectors{ get; set;}
    [Export] public RichTextLabel BidInfo { get; set;}
    [Export] public HSlider PoliticalSlider { get; set;}

    public void UpdateDisplay(Bid bid)
    {
        CompanyName.Text = bid.CompanyBidding.Name;
        CompanyInfo.Text = bid.CompanyBidding.Description;
        CompanySectors.Text = bid.CompanyBidding.Sectors.ToString();
        CompanyLogo.Texture = bid.CompanyBidding.Logo;
        BidInfo.Text = $"${bid.Price}";
        PoliticalSlider.Value = bid.BlockData.PoliticalLeaning;
    }
}
