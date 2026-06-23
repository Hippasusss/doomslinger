using Godot;

namespace DoomSlinger;

public partial class CorporateSection : Panel, ISection
{
    [Export] private SectionRevealer revealer;
    [Export] private Panel notificationLight;
    [Export] private Label CreditAmountDisplay;

    public void Toggle() => revealer?.Toggle();
    public void SetOpen(bool open) => revealer?.SetOpen(open);
    public bool IsOpen => revealer?.IsOpen ?? false;

    private int creditAmount;
    public int CreditAmount
    {
        get { return creditAmount; }
        set
        {
            creditAmount = value;
            CreditAmountDisplay.Text = creditAmount.ToString();
        }
    }

}
