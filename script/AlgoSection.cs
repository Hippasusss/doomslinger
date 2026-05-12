using System.Collections.Generic;
using Godot;

namespace DoomSlinger;

public partial class AlgoSection : Panel
{
    [Export] private ButtonMatrix bidMatrix;
    private Queue<Bid> bids = new();

    private readonly DeltaTimer bidUpdateTimer = new (1,5);
    public override void _Process(double delta)
    {
        if(bidUpdateTimer.Delta(delta))
        {

        }
    }

    private void GenerateNewBid()
    {
        // Bid bid = new();
    }
}
