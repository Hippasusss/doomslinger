using Godot;

public partial class MapDisplay : Control, IDisplay
{
    [Export] private SubViewport mapViewport;
    [Export] private TextureRect mapPreview;
    [Export] private Button mapButton;
    [Export] private SectionRevealer mapSectionToggle;

    public bool Enabled {get; set;} = true;

    public void ToggleOnOff(bool onOff)
    {
        Visible = onOff;
        Enabled = onOff;
    }

    public void UpdateDisplay(Human human)
    {
    }

    public override void _Ready()
    {
        if (mapViewport != null && mapPreview != null)
        {
            mapPreview.Texture = mapViewport.GetTexture();
        }

        mapButton.Pressed += OnClicked;
    }

    private void OnClicked()
    {
        mapSectionToggle?.Toggle();
    }
}
