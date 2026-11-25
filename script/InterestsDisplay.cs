using System.Collections.Generic;
using Godot;

namespace DoomSlinger;

public partial class InterestsDisplay : HFlowContainer, IDisplay
{
	[Export] private PackedScene coloredLabelScene;
	[Export] private bool centerRows = false;

	public bool Enabled { get; set; } = true;

	private float centerX;

	public override void _EnterTree()
	{
		// Capture the center authored in the scene before the container can
		// grow to fit its children.
		centerX = (OffsetLeft + OffsetRight) / 2f;
	}

	public override void _Ready()
	{
		if (centerRows) SortChildren += CenterRows;
	}

	public void UpdateDisplay(Human human)
	{
		UpdateDisplay(human.Data.interests);
	}

	public void UpdateDisplay(ContentType[] contentTypes)
	{
		if (!Enabled) return;
		ClearLabels();
		foreach (ContentType type in contentTypes)
		{
			ColoredLabel label = coloredLabelScene.Instantiate<ColoredLabel>();
			AddChild(label);
			label.Text = type.Name;
			label.SetBackgroundColour(type.Category.Color);
		}
	}

	public void ToggleOnOff(bool onOff)
	{
		if (!onOff) ClearLabels();
		Enabled = onOff;
	}

	private void ClearLabels()
	{
		foreach (Node child in GetChildren())
			child.QueueFree();
	}

	// FlowContainer grows rightwards from its left offset when children are
	// wider than it, and only applies its alignment to lines that fit. Keep
	// the container centered on the feedblock, then center each row so
	// overflowing rows poke out equally on both sides.
	private void CenterRows()
	{
		if (!IsVisibleInTree() || Size.X <= 0) return;

		Position = new Vector2(centerX - Size.X / 2f, Position.Y);

		Dictionary<int, (float Min, float Max)> rows = [];

		foreach (Node child in GetChildren())
		{
			if (child is not Control label || label.IsQueuedForDeletion() || !label.Visible) continue;

			int row = Mathf.RoundToInt(label.Position.Y);
			float left = label.Position.X;
			float right = left + label.Size.X;

			if (rows.TryGetValue(row, out (float Min, float Max) span))
			{
				rows[row] = (Mathf.Min(span.Min, left), Mathf.Max(span.Max, right));
			}
			else
			{
				rows[row] = (left, right);
			}
		}

		foreach (Node child in GetChildren())
		{
			if (child is not Control label || label.IsQueuedForDeletion() || !label.Visible) continue;

			int row = Mathf.RoundToInt(label.Position.Y);
			if (!rows.TryGetValue(row, out (float Min, float Max) span)) continue;

			float shift = Size.X / 2f - (span.Min + span.Max) / 2f;
			label.Position += new Vector2(shift, 0);
		}
	}
}
