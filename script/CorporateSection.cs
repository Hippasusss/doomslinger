using Godot;

namespace DoomSlinger;

public partial class CorporateSection : Panel, ISection
{
    [Export] private SectionRevealer revealer;
    [Export] private Panel notificationLight;
    [Export] private Label CreditAmountDisplay;
    [Export] private CutoutContainer cutoutContainer;

    private ShaderMaterial cutoutMaterial;

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

    public override void _Ready()
    {
        var shader = ResourceLoader.Load<Shader>("res://script/shader/cutout.gdshader");
        cutoutMaterial = new ShaderMaterial { Shader = shader };
        Material = cutoutMaterial;
        UpdateCutoutUniform();
    }

    private void UpdateCutoutUniform()
    {
        var size = GetRect().Size;
        var cutouts = cutoutContainer?.GetCutoutUVs(size) ?? new Vector4[8];

        for (int i = 0; i < cutouts.Length; i++)
            cutoutMaterial.SetShaderParameter($"cutout_{i}", cutouts[i]);

        if (GetThemeStylebox("panel") is StyleBoxFlat stylebox)
        {
            cutoutMaterial.SetShaderParameter("border_color", stylebox.BorderColor);
            cutoutMaterial.SetShaderParameter("border_width_uv", stylebox.BorderWidthLeft / size.X);
        }
    }
}
