using Godot;

public partial class Outline : Sprite2D
{
    [Export] public Color OutlineColor = Colors.Yellow;
    [Export] public float Width = 1.0f;
    [Export] public bool IsHighlighted = false;

    private Sprite2D _outlineSprite;
    private ShaderMaterial _outlineMaterial;

    public override void _Ready()
    {
        // 1. Create a "Ghost" sprite to hold the outline
        _outlineSprite = new Sprite2D();
        _outlineSprite.Texture = Texture;
        _outlineSprite.Centered = Centered;
        _outlineSprite.Offset = Offset;
        
        // Ensure it stays behind the parent
        _outlineSprite.ShowBehindParent = true;
        
        // 2. Setup the Material
        _outlineMaterial = new ShaderMaterial();
        _outlineMaterial.Shader = GD.Load<Shader>("res://script/shader/outline.gdshader");
        _outlineMaterial.SetShaderParameter("outline_color", OutlineColor);
        _outlineMaterial.SetShaderParameter("width", Width);

        _outlineSprite.Material = _outlineMaterial;
        _outlineSprite.Visible = IsHighlighted;
         
        // 3. Add as child so it follows this sprite
        AddChild(_outlineSprite);
        SetHighlighted(true);
    }

    public override void _Process(double delta)
    {
        // Keep the outline texture in sync if the parent texture changes
        if (_outlineSprite.Visible && _outlineSprite.Texture != Texture)
        {
            _outlineSprite.Texture = Texture;
        }
    }

    public void SetHighlighted(bool enabled)
    {
        IsHighlighted = enabled;
        if (_outlineSprite != null)
        {
            _outlineSprite.Visible = enabled;
            if (enabled) 
            {
                _outlineSprite.Texture = Texture;
            }
        }
    }
}
