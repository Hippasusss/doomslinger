using Godot;

namespace DoomSlinger;

public partial class MapDisplay : Control, IDisplay
{
    [Export] private TextureRect mapPreview;
    [Export] private Button mapButton;

    private MapSection mapSection;
    public bool Enabled { get; set; } = true;

    public void Init(MapSection section)
    {
        mapSection = section;
        if (mapPreview != null)
            mapPreview.Texture = section?.MapTexture;
    }

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
        mapButton.Pressed += OnClicked;
    }

    private void OnClicked()
    {
        mapSection?.Toggle();
    }
}
