using Godot;
using System;

namespace DoomSlinger;

public partial class BidDataDisplay : Panel
{
    [Export] public TextureRect CompanyLogo { get; set;}
    [Export] public RichTextLabel CompanyName { get; set;}
    [Export] public RichTextLabel CompanyInfo { get; set;}
    [Export] public RichTextLabel CompanySectors{ get; set;}
    [Export] public LabeledValue CompanyMarketCap { get; set;}
    [Export] public LabeledValue CompanyFoundedDate { get; set;}
    [Export] public LabeledValue CompanyStanding { get; set;}
    [Export] public LabeledValue BidPrice { get; set;}
    [Export] public LabeledValue BidLength { get; set;}
    [Export] public HSlider PoliticalSlider { get; set;}

    public void UpdateDisplay(Bid bid)
    {
        CompanyName.Text = bid.CompanyBidding.Name;
        CompanyInfo.Text = bid.CompanyBidding.Description;
        CompanySectors.Text = bid.CompanyBidding.Sectors.ToString();
        CompanyLogo.Texture = bid.CompanyBidding.Logo;
        CompanyMarketCap.ValueText = $"{bid.CompanyBidding.MarketCap}";
        CompanyFoundedDate.ValueText = $"{Mathf.FloorToInt(DateTime.Now.Year - bid.CompanyBidding.Age)}";
        CompanyStanding.ValueText = $"{bid.CompanyBidding.CurrentStanding}";
        BidPrice.ValueText = $"${bid.Price}";
        BidLength.ValueText = $"{bid.BlockData.Length}s";
        PoliticalSlider.Value = bid.BlockData.PoliticalLeaning;
    }
}
