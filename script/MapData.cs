using Godot;
using Godot.Collections;
using System.Collections.Generic;

[Tool]
[GlobalClass]
public partial class MapData : Resource
{
    private const float BrightnessThreshold = 0.35f;

    [Export] public Texture2D DisplayTexture { get; set;}
    private Texture2D navigationTexture;
    [Export] public Texture2D NavigationTexture
    {
        get => navigationTexture;
        set
        {
            navigationTexture = value;
            if (value != null)
                BuildFromTexture();
        }
    }
    [Export] public Array<Vector2> Points { get; set; }
    [Export] public Array<Vector2I> Edges { get; set; }
    [Export] private float distanceThreshold = 0.5f;
    [Export] private float proximityThreshold = 2f;

    [Export]
    public bool Rebuild
    {
        get => false;
        set
        {
            if (value && NavigationTexture != null)
            {
                BuildFromTexture();
                if (!string.IsNullOrEmpty(ResourcePath))
                    ResourceSaver.Save(this, ResourcePath);
            }
        }
    }

    [Export]
    public bool ShowDebug
    {
        get => false;
        set
        {
            if (!value || NavigationTexture == null || Points == null) return;

            Window window = new()
            {
                Title = "Navigation Debug",
                Unresizable = false
            };

            NavDebugDrawer drawer = new();
            drawer.SetData(this);
            window.AddChild(drawer);

            Vector2 texSize = NavigationTexture.GetSize();
            Vector2I windowSize = new((int)texSize.X, (int)texSize.Y);

            window.CloseRequested += () => window.QueueFree();

            EditorInterface.Singleton.GetBaseControl().AddChild(window);
            window.PopupCentered(windowSize);
        }
    }

    public void BuildFromTexture()
    {
        if (NavigationTexture == null) return;
        Image sourceImage = NavigationTexture.GetImage();
        if (sourceImage == null || sourceImage.GetWidth() == 0 || sourceImage.GetHeight() == 0) return;

        // Image info
        Vector2I imageSize = new(sourceImage.GetWidth(), sourceImage.GetHeight());
        System.Collections.Generic.Dictionary<Vector2I, int> pixelToId = [];
        Vector2 halfImageSize = new(imageSize.X / 2.0f, imageSize.Y / 2.0f);
        AStar2D graph = new();
        int pointId = 0;

        // Scan pixels → points
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

        // 8-connectivity
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

        // Reduce graph — alternate prune and merge until fixpoint
        bool reduced = true;
        while (reduced)
        {
            reduced = false;

            // Prune 2-connected nodes near the line between their neighbors
            while (true)
            {
                List<(long id, long neighborA, long neighborB)> toRemove = [];
                foreach (long id in graph.GetPointIds())
                {
                    long[] connections = graph.GetPointConnections(id);
                    if (connections.Length != 2) continue;
                    Vector2 posA = graph.GetPointPosition(connections[0]);
                    Vector2 posB = graph.GetPointPosition(id);
                    Vector2 posC = graph.GetPointPosition(connections[1]);
                    Vector2 ac = posC - posA;
                    float lenSq = ac.LengthSquared();
                    if (lenSq < 0.0001f) continue;
                    float t = Mathf.Clamp((posB - posA).Dot(ac) / lenSq, 0f, 1f);
                    if (posB.DistanceTo(posA + t * ac) > distanceThreshold) continue;
                    toRemove.Add((id, connections[0], connections[1]));
                }
                if (toRemove.Count == 0) break;
                reduced = true;
                foreach (var (id, queuedA, queuedB) in toRemove)
                {
                    long[] connections = graph.GetPointConnections(id);
                    if (connections.Length != 2) continue;
                    long a = connections[0], b = connections[1];
                    if ((a != queuedA || b != queuedB) && (a != queuedB || b != queuedA)) continue;
                    graph.RemovePoint(id);
                    if (!graph.ArePointsConnected(a, b))
                        graph.ConnectPoints(a, b);
                }
            }

            // Merge connected nodes closer than threshold (any degree)
            System.Collections.Generic.Dictionary<long, int> mergeCount = [];
            foreach (long id in graph.GetPointIds())
                mergeCount[id] = 1;

            while (true)
            {
                bool merged = false;
                foreach (long id in graph.GetPointIds())
                {
                    long[] connections = graph.GetPointConnections(id);
                    foreach (long conn in connections)
                    {
                        if (conn <= id) continue;
                        Vector2 posA = graph.GetPointPosition(id);
                        Vector2 posB = graph.GetPointPosition(conn);
                        if (posA.DistanceTo(posB) > proximityThreshold) continue;

                        int degA = graph.GetPointConnections(id).Length;
                        int degB = graph.GetPointConnections(conn).Length;
                        long survivor = degB > degA ? conn : (degA > degB ? id : (id < conn ? id : conn));
                        long victim = survivor == id ? conn : id;

                        int ca = mergeCount[survivor];
                        int cb = mergeCount[victim];
                        float total = ca + cb;
                        Vector2 newPos = (graph.GetPointPosition(survivor) * ca + graph.GetPointPosition(victim) * cb) / total;
                        mergeCount[survivor] = ca + cb;
                        mergeCount.Remove(victim);

                        long[] victimConns = graph.GetPointConnections(victim);
                        graph.RemovePoint(victim);
                        graph.SetPointPosition(survivor, newPos);
                        foreach (long vc in victimConns)
                        {
                            if (vc == survivor) continue;
                            if (!graph.ArePointsConnected(survivor, vc))
                                graph.ConnectPoints(survivor, vc);
                        }

                        merged = true;
                        break;
                    }
                    if (merged) break;
                }
                if (!merged) break;
                reduced = true;
            }
        }

        // Compact IDs to 0..n-1
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

        // Persist to export arrays
        SerializeFromGraph(graph);
    }

    public void LoadIntoGraph(AStar2D graph)
    {
        graph.Clear();
        for (int i = 0; i < Points.Count; i++)
            graph.AddPoint(i, Points[i]);
        for (int i = 0; i < Edges.Count; i++)
            graph.ConnectPoints(Edges[i].X, Edges[i].Y);
    }

    private void SerializeFromGraph(AStar2D graph)
    {
        Points = new Array<Vector2>();
        Edges = new Array<Vector2I>();

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
                    Edges.Add(new Vector2I(from, to));
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
