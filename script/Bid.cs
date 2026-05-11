using System;

public class Bid 
{
    private Company company;
    private BlockData data;
    private int price;

    public Bid(Company newCompany, BlockData newData, int newPrice)
    {
        company = newCompany;
        data = newData;
        price = newPrice;
    }
}

