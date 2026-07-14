using Godot;
using System.Collections.Generic;

public partial class OrdersManager : Panel
{

    [Export] PackedScene NewOrder;
    [Export] VBoxContainer OrderContainer;

    private List<Panel> Orders;

    public override void _Ready()
    {
        AddOrder();
    }

    public override void _Process(double delta)
    {
    }

    public void AddOrder()
    {
        Panel orderPanel = NewOrder.Instantiate<Panel>();
        OrderContainer.AddChild(orderPanel);
        Orders.Add(orderPanel);
    }

    public void CheckViolation()
    {
    }

    public void IssueViolation()
    {
    }

}
