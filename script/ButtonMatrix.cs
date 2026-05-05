using Godot;

[Tool]
public partial class ButtonMatrix : Panel
{
    private GridContainer grid;

    private int columns = 1;
    private int rows = 1;
    private PackedScene buttonScene;

    [Export]
    public int Columns
    {
        get => columns;
        set
        {
            columns = Mathf.Max(1, value);
            Rebuild();
        }
    }

    [Export]
    public int Rows
    {
        get => rows;
        set
        {
            rows = Mathf.Max(1, value);
            Rebuild();
        }
    }

    [Export]
    public PackedScene ButtonScene
    {
        get => buttonScene;
        set
        {
            buttonScene = value;
            Rebuild();
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
            grid.SetAnchorsPreset(LayoutPreset.FullRect);
            AddChild(grid);
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
        }
    }
}
