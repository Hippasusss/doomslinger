







using Godot;
using System;

public partial class IDDisplay : Node2D
{
    [Export] private RichTextLabel text;
    [Export] private Sprite2D face;
    public void UpdateID(Human human)
    {
        string displayText = $"""
            [table=2]
            [cell][b]NAME:[/b][/cell] [cell expand=true][right]{human.data.name}[/right][/cell]
            [cell][b]DOB:[/b][/cell]  [cell expand=true][right]{human.data.DOB}[/right][/cell]
            [cell][b]HT:[/b][/cell]   [cell expand=true][right]{human.data.height}[/right][/cell]
            [cell][b]GEN:[/b][/cell]  [cell expand=true][right]{human.data.gender}[/right][/cell]
            [cell][b]UID:[/b][/cell]  [cell expand=true][right]{human.data.UID}[/right][/cell]
            [cell][b]NAT:[/b][/cell]  [cell expand=true][right]{human.data.nationality}[/right][/cell]
            [/table]
            """;

        text.Text = displayText;

        face.Texture = human.Face.Texture;
    }

}
