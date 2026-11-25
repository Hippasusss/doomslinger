using Godot;

namespace DoomSlinger;

/// <summary>
/// A logic child that fades and moves its parent. Attach it under any
/// Node2D or Control and call Start()/Stop() from the parent when needed.
/// </summary>
public partial class FadeAndMoveParent : Node
{
	private float moveRate = 15f;
	private float fadeRate = 0.3f;
	[Export] private float horizontalDriftRange = 1f;
	private Vector2 resetPosition;
	private float driftDirection;

	private CanvasItem target;

	public bool IsIdle { get; private set; } = true;

	public override void _Ready()
	{
		target = GetParent() as CanvasItem;
		if (target == null)
		{
			GD.PushError($"{Name} must be a child of a Node2D or Control.");
			SetProcess(false);
			return;
		}

		resetPosition = GetPosition();
		target.Visible = false;
	}

	public override void _Process(double delta)
	{
		if (IsIdle || target == null) return;

		SetPosition(GetPosition() + new Vector2(driftDirection, -1) * (float)delta * moveRate);
		target.Modulate = new Color(target.Modulate, target.Modulate.A - fadeRate * (float)delta);

		if (target.Modulate.A <= 0)
		{
			Stop();
		}
	}

	public void Start()
	{
		if (target == null) return;

		IsIdle = false;
		driftDirection = (GD.Randf() * 2f - 1f) * horizontalDriftRange;
		target.Visible = true;
		target.Modulate = new Color(target.Modulate, 1);
		SetPosition(resetPosition);
	}

	public void Stop()
	{
		IsIdle = true;
		if (target != null) target.Visible = false;
	}

	private Vector2 GetPosition() => target switch
	{
		Node2D node2D => node2D.Position,
		Control control => control.Position,
		_ => Vector2.Zero,
	};

	private void SetPosition(Vector2 position)
	{
		switch (target)
		{
			case Node2D node2D:
				node2D.Position = position;
				break;
			case Control control:
				control.Position = position;
				break;
		}
	}
}
