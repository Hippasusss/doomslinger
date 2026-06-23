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
    [Export] public LabeledValue CompanyLocation{ get; set;}
    [Export] public LabeledValue BidPrice { get; set;}
    [Export] public LabeledValue BidLength { get; set;}
    [Export] public HSlider PoliticalSlider { get; set;}
    [Export] private Node contentLabels;
    [Export] private PackedScene coloredLabelScene;

    public void UpdateDisplay(Bid bid)
    {
        CompanyName.Text = bid.CompanyBidding.Name;
        CompanyInfo.Text = bid.CompanyBidding.Description;
        CompanySectors.Text = bid.CompanyBidding.Sectors.ToString();
        CompanyLogo.Texture = bid.CompanyBidding.Logo;
        CompanyMarketCap.ValueText = Helpers.FormatLargeDollarAmount(bid.CompanyBidding.MarketCap);
        CompanyFoundedDate.ValueText = $"{Mathf.FloorToInt(DateTime.Now.Year - bid.CompanyBidding.Age)}";
        CompanyStanding.ValueText = $"{bid.CompanyBidding.CurrentStanding}";
        CompanyLocation.ValueText = $"{bid.CompanyBidding.Nationality.Name}";
        BidPrice.ValueText = $"${bid.BlockData.Price}";
        BidLength.ValueText = $"{bid.BlockData.Length}s";
        PoliticalSlider.Value = bid.BlockData.PoliticalLeaning;
        DisplayContentLabels(bid.BlockData.ContentTypes);
    }

    private void DisplayContentLabels(ContentType[] types)
    {
        foreach (Node child in contentLabels.GetChildren())
            child.QueueFree();

        foreach (ContentType type in types)
        {
            ColoredLabel label = coloredLabelScene.Instantiate<ColoredLabel>();
            contentLabels.AddChild(label);
            label.Text = type.Name;
            label.SetBackgroundColour(type.Category.Color);
        }
    }
}
