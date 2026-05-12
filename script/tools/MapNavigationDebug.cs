using Godot;
using Godot.Collections;

namespace DoomSlinger;

[Tool]
public partial class NavDebugDrawer : Control
{
    private Texture2D texture;
    private MapData data;

    private float zoom = 1f;
    private Vector2 panOffset;
    private bool dragging;
    private Vector2 dragStart;

    public void SetData(MapData navData)
    {
        data = navData;
        texture = navData.NavigationTexture;
        MouseFilter = MouseFilterEnum.Stop;
        SetAnchorsPreset(LayoutPreset.FullRect);
        QueueRedraw();
    }

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mb)
        {
            if (mb.ButtonIndex == MouseButton.Left)
            {
                dragging = mb.Pressed;
                dragStart = mb.Position;
            }
            else if (mb.ButtonIndex == MouseButton.WheelUp && mb.Pressed)
            {
                ZoomAtPoint(mb.Position, 1.15f);
            }
            else if (mb.ButtonIndex == MouseButton.WheelDown && mb.Pressed)
            {
                ZoomAtPoint(mb.Position, 1f / 1.15f);
            }
        }
        else if (@event is InputEventMouseMotion mm && dragging)
        {
            panOffset += mm.Position - dragStart;
            dragStart = mm.Position;
            QueueRedraw();
        }
    }

    private void ZoomAtPoint(Vector2 cursorScreen, float factor)
    {
        float newZoom = Mathf.Clamp(zoom * factor, 0.05f, 20f);
        if (Mathf.IsEqualApprox(newZoom, zoom)) return;

        Vector2 center = Size / 2f;
        Vector2 worldAtCursor = (cursorScreen - center - panOffset) / zoom;
        zoom = newZoom;
        panOffset = cursorScreen - center - worldAtCursor * zoom;
        QueueRedraw();
    }

    public override void _Draw()
    {
        if (texture == null || data == null) return;

        Vector2 texSize = texture.GetSize();
        DrawSetTransform(panOffset + Size / 2f, 0f, Vector2.One * zoom);

        DrawTexture(texture, -texSize / 2f);

        Array<Vector2I> edges = data.Edges;
        Array<Vector2> points = data.Points;

        for (int i = 0; i < edges.Count; i++)
        {
            Vector2I edge = edges[i];
            if (edge.X >= points.Count || edge.Y >= points.Count) continue;
            DrawLine(points[edge.X], points[edge.Y], Colors.Red, 2f / zoom);
        }

        for (int i = 0; i < points.Count; i++)
            DrawCircle(points[i], 3f / zoom, Colors.Yellow);
    }
}
