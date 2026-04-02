







using Godot;

public partial class IDDisplay : Control, IDisplay
{
    [Export] private RichTextLabel textName;
    [Export] private RichTextLabel textData;
    [Export] private Sprite2D face;

    public bool Enabled {get; set;} = true;

    public void UpdateDisplay(Human human)
    {
        if(!Enabled) return;
        string nameText = $"NAME: [color=white][font_size=8]{human.Data.name}[/font_size][/color]";
        string dataText = $"""
            [table=2]
            [cell][right][b]DOB:[/b][/right][/cell]  [cell expand=true][color=white][right]{human.Data.DOB}[/right][/color][/cell]
            [cell][right][b]HT:[/b][/right][/cell]   [cell expand=true][color=white][right]{human.Data.height}[/right][/color][/cell]
            [cell][right][b]GEN:[/b][/right][/cell]  [cell expand=true][color=white][right]{human.Data.gender}[/right][/color][/cell]
            [cell][right][b]UID:[/b][/right][/cell]  [cell expand=true][color=white][right]{human.Data.UID}[/right][/color][/cell]
            [cell][right][b]NAT:[/b][/right][/cell]  [cell expand=true][color=white][right]{human.Data.nationality}[/right][/color][/cell]
            [/table]
            """;

        textName.Text = nameText;
        textData.Text = dataText;

        face.Texture = human.Face.Texture;
    }

    public void ToggleOnOff(bool onOff)
    {
        if (!onOff)
        {
            string displayText = $"""
                [table=2]
                [cell][b]DOB:[/b][/cell]  [cell expand=true][right]no data[/right][/cell]
                [cell][b]HT:[/b][/cell]   [cell expand=true][right]no data[/right][/cell]
                [cell][b]GEN:[/b][/cell]  [cell expand=true][right]no data[/right][/cell]
                [cell][b]UID:[/b][/cell]  [cell expand=true][right]no data[/right][/cell]
                [cell][b]NAT:[/b][/cell]  [cell expand=true][right]no data[/right][/cell]
                [/table]
                """;

            textData.Text = displayText;

            face.Texture = null;
        }
        Enabled = onOff;
    }
}
