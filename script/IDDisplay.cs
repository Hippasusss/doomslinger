







using Godot;
using System;

public partial class IDDisplay : Control, IDisplay
{
    [Export] private RichTextLabel text;
    [Export] private Sprite2D face;

    public bool Enabled {get; set;} = true;

    public void UpdateDisplay(Human human)
    {
        if(!Enabled) return;
        string displayText = $"""
            [table=2]
            [cell][right][b]NAME:[/b][/right][/cell] [cell expand=true][right]{human.Data.name}[/right][/cell]
            [cell][right][b]DOB:[/b][/right][/cell]  [cell expand=true][right]{human.Data.DOB}[/right][/cell]
            [cell][right][b]HT:[/b][/right][/cell]   [cell expand=true][right]{human.Data.height}[/right][/cell]
            [cell][right][b]GEN:[/b][/right][/cell]  [cell expand=true][right]{human.Data.gender}[/right][/cell]
            [cell][right][b]UID:[/b][/right][/cell]  [cell expand=true][right]{human.Data.UID}[/right][/cell]
            [cell][right][b]NAT:[/b][/right][/cell]  [cell expand=true][right]{human.Data.nationality}[/right][/cell]
            [/table]
            """;

        text.Text = displayText;

        face.Texture = human.Face.Texture;
    }

    public void ToggleOnOff(bool onOff)
    {
        if (!onOff)
        {
            string displayText = $"""
                [table=2]
                [cell][b]NAME:[/b][/cell] [cell expand=true][right]no data[/right][/cell]
                [cell][b]DOB:[/b][/cell]  [cell expand=true][right]no data[/right][/cell]
                [cell][b]HT:[/b][/cell]   [cell expand=true][right]no data[/right][/cell]
                [cell][b]GEN:[/b][/cell]  [cell expand=true][right]no data[/right][/cell]
                [cell][b]UID:[/b][/cell]  [cell expand=true][right]no data[/right][/cell]
                [cell][b]NAT:[/b][/cell]  [cell expand=true][right]no data[/right][/cell]
                [/table]
                """;

            text.Text = displayText;

            face.Texture = null;
        }
        Enabled = onOff;
    }
}
