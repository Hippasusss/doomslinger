using Godot;
using Godot.Collections;
using System.Collections.Generic;

[Tool]
[GlobalClass]
public partial class NavigationGraphData : Resource
{
    private const float BrightnessThreshold = 0.35f;

    private Texture2D sourceTexture;
    [Export] public Texture2D SourceTexture
    {
        get => sourceTexture;
        set
        {
            sourceTexture = value;
            if (value != null)
                BuildFromTexture();
        }
    }
    [Export] public Array<Vector2> Points { get; set; }
    [Export] public Array<int> EdgesFrom { get; set; }
    [Export] public Array<int> EdgesTo { get; set; }

    public void BuildFromTexture()
    {
        if (SourceTexture == null) return;
        Image sourceImage = SourceTexture.GetImage();
        if (sourceImage == null || sourceImage.GetWidth() == 0 || sourceImage.GetHeight() == 0) return;

        int imageWidth = sourceImage.GetWidth();
        int imageHeight = sourceImage.GetHeight();
        Vector2I imageSize = new(imageWidth, imageHeight);
        System.Collections.Generic.Dictionary<Vector2I, int> pixelToId = [];
        Vector2 halfImageSize = new(imageSize.X / 2.0f, imageSize.Y / 2.0f);
        AStar2D graph = new();
        int pointId = 0;

        bool IsLineWalkable(Vector2 from, Vector2 to)
        {
            int x0 = Mathf.FloorToInt(from.X + imageWidth / 2.0f);
            int y0 = Mathf.FloorToInt(from.Y + imageHeight / 2.0f);
            int x1 = Mathf.FloorToInt(to.X + imageWidth / 2.0f);
            int y1 = Mathf.FloorToInt(to.Y + imageHeight / 2.0f);
            int dx = Mathf.Abs(x1 - x0), dy = Mathf.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1, sy = y0 < y1 ? 1 : -1;
            int err = dx - dy;
            while (true)
            {
                if (!IsWalkablePixel(sourceImage.GetPixel(x0, y0)))
                    return false;
                if (x0 == x1 && y0 == y1) break;
                int e2 = 2 * err;
                if (e2 > -dy) { err -= dy; x0 += sx; }
                if (e2 < dx) { err += dx; y0 += sy; }
            }
            return true;
        }

        for (int y = 0; y < imageSize.Y; y++)
        {
            for (int x = 0; x < imageSize.X; x++)
            {
                if (!IsWalkablePixel(sourceImage.GetPixel(x, y))) continue;
                graph.AddPoint(pointId, new Vector2(x + 0.5f - halfImageSize.X, y + 0.5f - halfImageSize.Y));
                pixelToId[new Vector2I(x, y)] = pointId;
                pointId++;
            }
        }

        Vector2I[] neighborOffsets = [
            new(-1, -1), new(0, -1), new(1, -1),
            new(-1,  0),              new(1,  0),
            new(-1,  1), new(0,  1), new(1,  1)
        ];

        foreach (KeyValuePair<Vector2I, int> point in pixelToId)
        {
            foreach (Vector2I offset in neighborOffsets)
            {
                Vector2I neighbor = point.Key + offset;
                if (!pixelToId.TryGetValue(neighbor, out int neighborId)) continue;
                if (graph.ArePointsConnected(point.Value, neighborId)) continue;
                graph.ConnectPoints(point.Value, neighborId);
            }
        }

        while (true)
        {
            List<long> toRemove = [];
            foreach (long id in graph.GetPointIds())
            {
                long[] connections = graph.GetPointConnections(id);
                if (connections.Length != 2) continue;
                if (!IsLineWalkable(graph.GetPointPosition(connections[0]), graph.GetPointPosition(connections[1]))) continue;
                toRemove.Add(id);
            }
            if (toRemove.Count == 0) break;
            foreach (long id in toRemove)
            {
                long[] connections = graph.GetPointConnections(id);
                if (connections.Length != 2) continue;
                long a = connections[0], b = connections[1];
                graph.RemovePoint(id);
                if (!graph.ArePointsConnected(a, b))
                    graph.ConnectPoints(a, b);
            }
        }

        long[] oldIds = graph.GetPointIds();
        if (oldIds.Length == 0) return;
        AStar2D compacted = new();
        System.Collections.Generic.Dictionary<long, int> idMap = [];
        for (int i = 0; i < oldIds.Length; i++)
        {
            compacted.AddPoint(i, graph.GetPointPosition(oldIds[i]));
            idMap[oldIds[i]] = i;
        }
        foreach (long oldId in oldIds)
        {
            foreach (long conn in graph.GetPointConnections(oldId))
            {
                int ni = idMap[oldId], nc = idMap[conn];
                if (!compacted.ArePointsConnected(ni, nc))
                    compacted.ConnectPoints(ni, nc);
            }
        }
        graph = compacted;

        bool changed = true;
        while (changed)
        {
            changed = false;
            int count = (int)graph.GetPointCount();
            for (int i = 0; i < count; i++)
            {
                Vector2 posI = graph.GetPointPosition(i);
                long[] neighbors = graph.GetPointConnections(i);
                for (int j = 0; j < neighbors.Length; j++)
                {
                    int neighbor = (int)neighbors[j];
                    long[] n2 = graph.GetPointConnections(neighbor);
                    for (int k = 0; k < n2.Length; k++)
                    {
                        int candidate = (int)n2[k];
                        if (candidate == i) continue;
                        if (graph.ArePointsConnected(i, candidate)) continue;
                        if (IsLineWalkable(posI, graph.GetPointPosition(candidate)))
                        {
                            graph.ConnectPoints(i, candidate);
                            changed = true;
                        }
                    }
                }
            }
        }

        SerializeFromGraph(graph);
    }

    public void LoadIntoGraph(AStar2D graph)
    {
        graph.Clear();
        for (int i = 0; i < Points.Count; i++)
            graph.AddPoint(i, Points[i]);
        for (int i = 0; i < EdgesFrom.Count; i++)
            graph.ConnectPoints(EdgesFrom[i], EdgesTo[i]);
    }

    private void SerializeFromGraph(AStar2D graph)
    {
        Points = new Array<Vector2>();
        EdgesFrom = new Array<int>();
        EdgesTo = new Array<int>();

        long[] ids = graph.GetPointIds();
        for (int i = 0; i < ids.Length; i++)
            Points.Add(graph.GetPointPosition(ids[i]));

        for (int i = 0; i < ids.Length; i++)
        {
            int from = (int)ids[i];
            foreach (long conn in graph.GetPointConnections(from))
            {
                int to = (int)conn;
                if (to > from)
                {
                    EdgesFrom.Add(from);
                    EdgesTo.Add(to);
                }
            }
        }
    }

    private static bool IsWalkablePixel(Color color)
    {
        if (color.A < 0.1f) return false;
        float brightness = Mathf.Max(color.R, Mathf.Max(color.G, color.B));
        return brightness <= BrightnessThreshold;
    }
}
