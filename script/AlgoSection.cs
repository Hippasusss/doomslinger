using System.Collections.Generic;
using Godot;

namespace DoomSlinger;

public partial class AlgoSection : Panel
{
    [Export] private GameData gameData;
    [Export] private ButtonMatrix bidMatrix;
    [Export] private BidDataDisplay bidDataDisplay;

    private readonly Dictionary<MatrixCell, Bid> ActiveBids = [];

    public override void _Ready()
    {
        for (int i = 0; i < bidMatrix.Count; i++)
        {
            MatrixCell cell = bidMatrix.GetCell(i);
            cell.Button.MouseEntered += () => DisplayCellData(cell);
        }
    }

    private readonly DeltaTimer BidGenTimer = new (8);
    public override void _Process(double delta)
    {
        if(BidGenTimer.Delta(delta))
        {
            GenerateBatchOfBids();
        }
    }

    private void GenerateBatchOfBids()
    {
        var cascade = CreateTween();
        int numberOfBids = GD.RandRange(1,10);
        Bid[] queuedBids = new Bid[numberOfBids];

        for(int i = 0; i < numberOfBids; i++)
        {
            queuedBids[i] = new(gameData.GetRandomCompany());
        }

        for (int i = 0; i < queuedBids.Length; i++)
        {
            Bid bid = queuedBids[i];
            MatrixCell cell;
            do { cell = bidMatrix.GetRandomCell(); } while (ActiveBids.ContainsKey(cell));
            ActiveBids.Add(cell, bid);
            cell.Text.Text = $"${bid.Price}";
            Color c = bid.BlockData.BlockColor;
            var capturedCell = cell;
            cascade.TweenInterval(0.07f);
            cascade.TweenCallback(Callable.From(() => capturedCell.Color = c));
        }
    }
    private void DisplayCellData(MatrixCell cell)
    {
        if(!ActiveBids.TryGetValue(cell, out Bid value)) return;
        bidDataDisplay.UpdateDisplay(value);
    }

}
