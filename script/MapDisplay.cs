using Godot;

public partial class MapDisplay : Control, IDisplay
{
    [Export] private SubViewport mapViewport;
    [Export] private TextureRect mapPreview;
    [Export] private Button mapButton;
    [Export] private SectionRevealer mapSectionToggle;

    public bool Enabled { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public void ToggleOnOff(bool onOff)
    {
        throw new System.NotImplementedException();
    }

    public void UpdateDisplay(Human human)
    {
        throw new System.NotImplementedException();
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
