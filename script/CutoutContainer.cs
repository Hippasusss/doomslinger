using Godot;

namespace DoomSlinger;

public partial class CutoutContainer : Control
{
    private const int MaxCutouts = 8;

    public Vector4[] GetCutoutUVs(Vector2 parentSize)
    {
        var result = new Vector4[MaxCutouts];
        int i = 0;
        foreach (Control child in GetChildren())
        {
            if (child is not Control) continue;
            var rect = child.GetRect();
            result[i] = new Vector4(
                rect.Position.X / parentSize.X,
                rect.Position.Y / parentSize.Y,
                rect.Size.X / parentSize.X,
                rect.Size.Y / parentSize.Y
            );
            i++;
            if (i >= MaxCutouts) break;
        }
        return result;
    }
}
