using Godot;
using System.Linq;
using System.Threading.Tasks;

[Tool]
public partial class FaceGenerator : SubViewport
{
    [Export] public float HairChance = 0.8f;
    [Export] public float BeardChance = 0.4f;
    [Export] public float GlassesChance = 0.15f;
    [Export] public ColorPalette facePalette;
    [Export] public ColorPalette hairPalette;
    [Export] public ColorPalette clotPalette;
    [Export] public ColorPalette trimPalette;
    [Export] public ColorPalette eyesPalette;
    [Export] public ColorPalette phonePalette;
    [Export] public ColorPalette jewleryPalette;
    [Export] public Texture2D maleSpriteSheet;
    [Export] public Texture2D femaleSpriteSheet;
    [Export] public bool ReGenerate { get => false; set { if (value) { 
        var Data = new HumanPersonalData("Danny Herbert", System.DateOnly.MaxValue, 160, "M", "Scottish", 3);
        _ = GenerateAsync(Data); 
    } } }

    private const float stretchCoefficient = 0.2f;


    public override void _Ready()
    {
        RenderTargetUpdateMode = UpdateMode.Always;
    }

    public async Task<(ImageTexture texture, Color[] colours)> GenerateAsync(HumanPersonalData data)
    {
        var children = GetChildren().OfType<FaceComponent>().ToList();

        Texture2D sheet;
        if (data.gender == "F")
        {
            sheet = femaleSpriteSheet;
        }
        else
        {
            sheet = maleSpriteSheet;
        }
        foreach(FaceComponent sprite in children)
        {
            sprite.Texture = sheet;
        }

        Color faceColor = facePalette.Colors.GetRandom();
        Color hairColor = hairPalette.Colors.GetRandom();
        Color clotColor = clotPalette.Colors.GetRandom();
        Color trimColor = trimPalette.Colors.GetRandom();
        Color eyeColor  = eyesPalette.Colors.GetRandom();
        Color phoneColor = phonePalette.Colors.GetRandom();
        Color jewleryColor = jewleryPalette.Colors.GetRandom();

        const float shift = 0.1f;
        float hueOffset = (float)GD.RandRange(-shift/2, shift/2);
        float satOffset = (float)GD.RandRange(-shift, shift); 
        float valOffset = (float)GD.RandRange(-shift, shift);

        faceColor.H = Mathf.Clamp(faceColor.H + hueOffset, 0f, 1f);
        faceColor.S = Mathf.Clamp(faceColor.S + satOffset, 0f, 1f);
        faceColor.V = Mathf.Clamp(faceColor.V + valOffset, 0f, 1f);

        for (int i = 0; i < children.Count; i++)
        {
            FaceComponent part = children[i];

            if (part.Material != null)
            {
                part.Material = (Material)part.Material.Duplicate();
            }

            int frame = GD.RandRange(0, part.Hframes - 1);
            part.Frame = part.StartFrame + frame;

            string name = part.Name.ToString().ToLower();
            part.Visible = true;

            if (name.Contains("hair") && !name.Contains("back"))
            {
                part.Visible = (GD.Randf() < HairChance) || data.gender == "F";
                part.Modulate = hairColor;
                var hairback = children.FirstOrDefault(x => x.Name.ToString().Equals("hairback", System.StringComparison.CurrentCultureIgnoreCase));
                hairback.Modulate = hairColor;
                hairback.Frame = hairback.StartFrame + frame;
                GD.Print(name);
                GD.Print(hairColor);
                GD.Print(hairback.Name);
            }
            else if (name.Contains("beard"))
            {
                part.Visible = GD.Randf() < BeardChance;
                part.Modulate = data.gender == "M" ? hairColor : jewleryColor;
            }
            else if (name.Contains("face"))
            {
                part.Modulate = faceColor;
            }
            else if (name.Contains("body"))
            {
                part.Modulate = clotColor;
            }
            else if (name.Contains("glasses"))
            {
                part.Visible = GD.Randf() < GlassesChance;
                part.Modulate = trimColor;
            }
            else if (name.Contains("eyes") || name.Contains("nose") || name.Contains("mouth"))
            {
                part.Modulate = faceColor;
            }
        }

        float clampedHeight = Mathf.Clamp(data.height, 140f, 210f);
        float stretchFactor = (clampedHeight - 140f) / 70f; 
        float scaleY = 1.0f + (stretchCoefficient * stretchFactor);

        CanvasTransform = new Transform2D(
            new Vector2(1f, 0f),  
            new Vector2(0f, scaleY),
            new Vector2(0f, Size.Y * (1f - scaleY))  
        );

        RenderTargetUpdateMode = UpdateMode.Always;

        // Wait for two frames (one to process the change, one to render it)
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

        Image image = GetTexture().GetImage();
        Color[] colors = [faceColor, hairColor, clotColor, trimColor, eyeColor, phoneColor];
        return (ImageTexture.CreateFromImage(image), colors);
    }
}
