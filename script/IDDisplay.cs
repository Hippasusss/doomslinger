







using Godot;
using System;

public partial class IDDisplay : Node2D
{
	[Export] private RichTextLabel text;
	[Export] private Sprite2D face;
	public void UpdateID(Human human)
	{
		string displayText = $""" 
		[b]NAME:[/b] {human.data.name}
		[b]DOB:[/b]  {human.data.DOB}
		[b]HT:[/b]   {human.data.height}
		[b]GEN:[/b]  {human.data.gender}
		[b]UID:[/b]  {human.data.UID}
		[b]NAT:[/b]  {human.data.nationality}
		""";

		text.Text = displayText;

		face.Texture = human.Face.Texture;
	}

}
