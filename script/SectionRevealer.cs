using Godot;
using System;

public partial class SectionRevealer : Node2D
{
	[Export] public AnimationPlayer animation;
	private bool open = false;

	public override void _Ready()
	{
	}

	public void Open()
	{
		animation.Play("Open");
		open = true;
	}

	public void Close()
	{

		animation.Play("Close");
		open = false;
	}

	public void OnClick(Node viewport, InputEvent clickEvent, long shape_idx)
	{
		if (clickEvent is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
		{
			if (mouseEvent.ButtonIndex == MouseButton.Left)
			{
				if(open) Close();
				else Open();
			}
		}
	}


}
