using Godot;

namespace DoomSlinger;

public partial class ButtonMatrix : Panel
{
    private GridContainer grid;

    private PackedScene buttonScene;
    private int columns = 5;
    private int rows = 6;
    private int margin = 2;
    private int gap = 1;

    [Export] public PackedScene ButtonScene { get => buttonScene; set => buttonScene = value; }
    [Export] public int Columns { get => columns; set => columns = value; }
    [Export] public int Rows { get => rows; set => rows = value; }
    [Export] public int Margin { get => margin; set => margin = value; }
    [Export] public int Gap { get => gap; set => gap = value; }

    public override void _Ready()
    {
        Rebuild();
    }

    private void Rebuild()
    {
        if (buttonScene == null) return;

        if (grid == null)
        {
            grid = new GridContainer();
            grid.SetAnchorsPreset(LayoutPreset.FullRect, true);
            ApplyMargins();
            ApplyGaps();
            AddChild(grid);
            grid.Owner = this;
        }

        foreach (Node child in grid.GetChildren())
            child.QueueFree();

        grid.Columns = columns;
        for (int i = 0; i < rows * columns; i++)
        {
            MatrixCell cell = buttonScene.Instantiate<MatrixCell>();
            cell.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            cell.SizeFlagsVertical = SizeFlags.ExpandFill;
            grid.AddChild(cell);
            cell.Owner = grid;
        }
    }

    private void ApplyMargins()
    {
        grid.OffsetLeft = margin;
        grid.OffsetTop = margin;
        grid.OffsetRight = -margin;
        grid.OffsetBottom = -margin;
    }

    private void ApplyGaps()
    {
        grid.AddThemeConstantOverride("h_separation", gap);
        grid.AddThemeConstantOverride("v_separation", gap);
    }

    public MatrixCell GetCell(int index)
    {
        if (grid == null || index < 0 || index >= grid.GetChildCount()) return null;
        return grid.GetChild(index) as MatrixCell;
    }

}
