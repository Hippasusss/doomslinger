using Godot;

public partial class CreditSection : Panel
{
    [Export] private RichTextLabel creditText;

    private int creditAmount;

    public int CreditAmount
    {
        get => creditAmount;
        private set
        {
            creditAmount += value;
            creditText.Text = creditAmount.ToString();
        }
    }
}
