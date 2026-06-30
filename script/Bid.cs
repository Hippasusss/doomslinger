namespace DoomSlinger;

public class Bid 
{
    public Company CompanyBidding { get; set; }
    public BlockData BlockData { get; set; }

    public Bid(Company newCompany, ContentType[] contentTypePool)
    {
        CompanyBidding = newCompany;
        BlockData = new(newCompany, contentTypePool);
    }


}
