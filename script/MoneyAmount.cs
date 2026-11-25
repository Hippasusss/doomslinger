using Godot;

namespace DoomSlinger;

public partial class MoneyAmount : RichTextLabel
{
	[Export] private FadeAndMoveParent animator;

	public bool IsIdle => animator == null || animator.IsIdle;

	public override void _Ready()
	{
		animator ??= GetNodeOrNull<FadeAndMoveParent>("FadeAndMoveParent");
		if (animator == null)
		{
			GD.PushError($"{Name} is missing its FadeAndMoveParent child.");
		}
	}

	public void Display(float amount)
	{
		Text = $"${amount:0.##}";
		animator?.Start();
	}
}
