
using System.Collections.Generic; using Godot;

namespace DoomSlinger;

public partial class AlgoSection : Panel, ISection
{
    [Export] private GameData gameData;
    [Export] private ButtonMatrix bidMatrix;
    [Export] private BidDataDisplay bidDataDisplay;
    [Export] private SectionRevealer revealer;

    public void Toggle() => revealer?.Toggle();
    public void SetOpen(bool open) => revealer?.SetOpen(open);
    public bool IsOpen => revealer?.IsOpen ?? false;

    private readonly Dictionary<MatrixCell, Bid> ActiveBids = [];
    private readonly Dictionary<MatrixCell, Human> CellOwners = [];
    private readonly Dictionary<Human, List<MatrixCell>> HumanSelections = [];

    private Human currentHuman;

    public override void _Ready()
    {
        for (int i = 0; i < bidMatrix.Count; i++)
        {
            MatrixCell cell = bidMatrix.GetCell(i);
            cell.Button.MouseEntered += () => HoverCell(cell);
            cell.Button.Pressed += () => ClickCell(cell);
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
    
    public void DisplayHuman(Human human)
    {
        currentHuman = human;
        UpdateAllCellDisplays();
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

    public Bid TryConsumeFirstBid(Human human)
    {
        if (!HumanSelections.TryGetValue(human, out var list) || list.Count == 0)
            return null;

        MatrixCell cell = list[0];
        Bid bid = ActiveBids[cell];

        list.RemoveAt(0);
        CellOwners.Remove(cell);
        ActiveBids.Remove(cell);
        cell.Reset();

        return bid;
    }

    // Shows company and bid data. registered in this constructor.
    private void HoverCell(MatrixCell cell)
    {
        if(!ActiveBids.TryGetValue(cell, out Bid value)) return;
        bidDataDisplay.UpdateDisplay(value);
    }

    // Adds cell bid to human upcoming bids and updates display. registered in this constructor.
    private void ClickCell(MatrixCell cell)
    {
        if (!ActiveBids.TryGetValue(cell, out Bid bid)) return;

        if (CellOwners.TryGetValue(cell, out Human owner) && owner == currentHuman)
        {
            CellOwners.Remove(cell);
            HumanSelections[currentHuman].Remove(cell);
        }
        else
        {
            if (owner != null)
                HumanSelections[owner].Remove(cell);
            CellOwners[cell] = currentHuman;
            if (!HumanSelections.TryGetValue(currentHuman, out var list))
                HumanSelections[currentHuman] = list = [];
            list.Add(cell);
        }

        UpdateAllCellDisplays();
    }

    private void UpdateAllCellDisplays()
    {
        foreach (var (cell, _) in ActiveBids)
        {
            if (CellOwners.TryGetValue(cell, out Human owner))
            {
                int index = HumanSelections[owner].IndexOf(cell) + 1;
                cell.HideSelectionNumber(false);
                cell.SetSelectionNumber(index, owner.ColorPhone);

                if(index == 1)
                {
                    cell.Flash(true);
                }
                else
                {
                    cell.Flash(false);
                }
            }
            else
            {
                cell.HideSelectionNumber(true);
            }
        }
    }
}
