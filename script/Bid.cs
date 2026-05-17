using Godot;

namespace DoomSlinger;

public class Bid 
{
    public Company CompanyBidding { get; set; }
    public BlockData BlockData { get; set; }
    public int Price { get; set;}

    public Bid(Company newCompany)
    {
        CompanyBidding = newCompany;
        BlockData = new(newCompany);
        Price = (int)GD.Randfn(5f, newCompany.Chaos);
    }


}
