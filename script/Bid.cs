using Godot;

namespace DoomSlinger;

public class Bid 
{
    public Company CompanyBidding { get; set; }
    public BlockData BlockData { get; set; }
    public int Price { get; set;}

    public Bid(Company newCompany, ContentType[] contentTypePool)
    {
        CompanyBidding = newCompany;
        BlockData = new(newCompany, contentTypePool);
        Price = (int)GD.Randfn(5f, newCompany.Chaos);
    }


}
