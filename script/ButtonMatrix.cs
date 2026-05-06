using Godot;

[Tool]
public partial class ButtonMatrix : Panel
{
    private GridContainer grid;

    private int columns = 1;
    private int rows = 1;
    private PackedScene buttonScene;
    private int margin = 2;
    private int gap = 0;

    [Export]
    public int Columns
    {
        get => columns;
        set
        {
            columns = Mathf.Max(1, value);
            if (IsInsideTree()) Rebuild();
        }
    }

    [Export]
    public int Rows
    {
        get => rows;
        set
        {
            rows = Mathf.Max(1, value);
            if (IsInsideTree()) Rebuild();
        }
    }

    [Export]
    public PackedScene ButtonScene
    {
        get => buttonScene;
        set
        {
            buttonScene = value;
            if (IsInsideTree()) Rebuild();
        }
    }

    [Export]
    public int Margin
    {
        get => margin;
        set
        {
            margin = value;
            if (grid != null) ApplyMargins();
        }
    }

    [Export]
    public int Gap
    {
        get => gap;
        set
        {
            gap = value;
            if (grid != null) ApplyGaps();
        }
    }

    public override void _Ready()
    {
        Rebuild();
    }

    private void Rebuild()
    {
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

        if (buttonScene == null || columns < 1 || rows < 1) return;

        grid.Columns = columns;
        for (int i = 0; i < rows * columns; i++)
        {
            Button button = buttonScene.Instantiate<Button>();
            button.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            button.SizeFlagsVertical = SizeFlags.ExpandFill;
            grid.AddChild(button);
            button.Owner = grid;
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

    public Button GetButton(int index)
    {
        if (grid == null || index < 0 || index >= grid.GetChildCount()) return null;
        return grid.GetChild(index) as Button;
    }
}
