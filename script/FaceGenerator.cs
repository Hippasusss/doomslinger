using Godot;
using System.Linq;
using System.Threading.Tasks;

[Tool]
public partial class FaceGenerator : SubViewport
{
    [Export] public float HairChance = 0.8f;
    [Export] public float BeardChance = 0.4f;
    [Export] public float GlassesChance = 0.15f;
    [Export] public bool ReGenerate { get => false; set { if (value) { _ = GenerateAsync(); } } }
    [Export] public ColorPalette facePalette;
    [Export] public ColorPalette hairPalette;
    [Export] public ColorPalette clotPalette;
    [Export] public ColorPalette trimPalette;
    [Export] public Texture2D maleSpriteSheet;
    [Export] public Texture2D femaleSpriteSheet;


    public override void _Ready()
    {
        RenderTargetUpdateMode = UpdateMode.Always;
    }

    public async Task<ImageTexture> GenerateAsync()
    {
        var children = GetChildren().OfType<Sprite2D>().ToList();

        Color faceColor = facePalette.Colors.GetRandom();
        Color hairColor = hairPalette.Colors.GetRandom();
        Color clotColor = clotPalette.Colors.GetRandom();
        Color trimColor = trimPalette.Colors.GetRandom();

        for (int i = 0; i < children.Count; i++)
        {
            Sprite2D part = children[i];

            if (part.Material != null)
            {
                part.Material = (Material)part.Material.Duplicate();
            }

            int variationsPerPart = 5;
            int startFrame = i * variationsPerPart;
            int endFrame = startFrame + variationsPerPart - 1;

            part.Frame = GD.RandRange(startFrame, endFrame);

            string name = part.Name.ToString().ToLower();
            part.Visible = true;
            
            if (part.Material is ShaderMaterial mat)
            {
                if (name.Contains("hair"))
                {
                    part.Visible = GD.Randf() < HairChance;
                    mat.SetShaderParameter("target_color", hairColor);
                }
                else if (name.Contains("beard"))
                {
                    part.Visible = GD.Randf() < BeardChance;
                    mat.SetShaderParameter("target_color", hairColor);
                }
                else if (name.Contains("face"))
                {
                    mat.SetShaderParameter("target_color", faceColor);
                }
                else if (name.Contains("body"))
                {
                    mat.SetShaderParameter("target_color", clotColor);
                }
                else if (name.Contains("glasses"))
                {
                    part.Visible = GD.Randf() < GlassesChance;
                    mat.SetShaderParameter("target_color", trimColor);
                }
                else if (name.Contains("eyes") || name.Contains("nose") || name.Contains("mouth"))
                {
                    mat.SetShaderParameter("target_color", faceColor);
                }
            }
            else if (name.Contains("hair"))
            {
                part.Visible = GD.Randf() < HairChance;
            }
            else if (name.Contains("beard"))
            {
                part.Visible = GD.Randf() < BeardChance;
            }
            else if (name.Contains("glasses"))
            {
                part.Visible = GD.Randf() < GlassesChance;
            }
        }

        RenderTargetUpdateMode = UpdateMode.Always;
        
        // Wait for two frames (one to process the change, one to render it)
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

        Image image = GetTexture().GetImage();
        return ImageTexture.CreateFromImage(image);
    }
}
