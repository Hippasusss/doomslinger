using Godot;
using System.Collections.Generic;

public partial class NavigationArea : Node
{
    private AStar2D walkableGraph = new();
    private Image sourceImage;
    private int imageWidth;
    private int imageHeight;

    private const float brightnessThreshold = 0.35f;

    public bool HasPoints => walkableGraph.GetPointCount() > 0;
    public int PointCount => (int)walkableGraph.GetPointCount();

    public void BuildFromTexture(Texture2D texture)
    {
        if (texture == null) return;

        sourceImage = texture.GetImage();
        if (sourceImage == null || sourceImage.GetWidth() == 0 || sourceImage.GetHeight() == 0) return;

        imageWidth = sourceImage.GetWidth();
        imageHeight = sourceImage.GetHeight();

        walkableGraph.Clear();

        Vector2I imageSize = new(imageWidth, imageHeight);
        Dictionary<Vector2I, int> pixelToId = [];
        Vector2 halfImageSize = new(imageSize.X / 2.0f, imageSize.Y / 2.0f);
        int pointId = 0;

        for (int y = 0; y < imageSize.Y; y++)
        {
            for (int x = 0; x < imageSize.X; x++)
            {
                if (!IsWalkablePixel(sourceImage.GetPixel(x, y))) continue;

                Vector2 position = new(x + 0.5f - halfImageSize.X, y + 0.5f - halfImageSize.Y);
                walkableGraph.AddPoint(pointId, position);
                pixelToId.Add(new(x, y), pointId);
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
                if (walkableGraph.ArePointsConnected(point.Value, neighborId)) continue;
                walkableGraph.ConnectPoints(point.Value, neighborId);
            }
        }

        ReduceGraph();
    }

    private void ReduceGraph()
    {
        while (true) //Prune
        {
            List<long> toRemove = [];

            foreach (long id in walkableGraph.GetPointIds())
            {
                long[] connections = walkableGraph.GetPointConnections(id);
                if (connections.Length != 2) continue;

                Vector2 posA = walkableGraph.GetPointPosition(connections[0]);
                Vector2 posB = walkableGraph.GetPointPosition(connections[1]);

                if (!IsLineWalkable(posA, posB)) continue;

                toRemove.Add(id);
            }

            if (toRemove.Count == 0) break;

            foreach (long id in toRemove)
            {
                long[] connections = walkableGraph.GetPointConnections(id);
                if (connections.Length != 2) continue;

                long a = connections[0];
                long b = connections[1];

                walkableGraph.RemovePoint(id);
                if (!walkableGraph.ArePointsConnected(a, b))
                    walkableGraph.ConnectPoints(a, b);
            }
        }

        //Compact
        long[] oldIds = walkableGraph.GetPointIds();
        if (oldIds.Length == 0) return;

        AStar2D compactedGraph = new();
        Dictionary<long, int> idMap = [];

        for (int i = 0; i < oldIds.Length; i++)
        {
            Vector2 pos = walkableGraph.GetPointPosition(oldIds[i]);
            compactedGraph.AddPoint(i, pos);
            idMap[oldIds[i]] = i;
        }

        foreach (long oldId in oldIds)
        {
            long[] connections = walkableGraph.GetPointConnections(oldId);
            int newId = idMap[oldId];
            foreach (long conn in connections)
            {
                int newConnId = idMap[conn];
                if (!compactedGraph.ArePointsConnected(newId, newConnId))
                    compactedGraph.ConnectPoints(newId, newConnId);
            }
        }

        walkableGraph = compactedGraph;

        bool changed = true;
        while (changed)
        {
            changed = false;
            int count = (int)walkableGraph.GetPointCount();
            for (int i = 0; i < count; i++)
            {
                Vector2 posI = walkableGraph.GetPointPosition(i);
                long[] neighbors = walkableGraph.GetPointConnections(i);

                for (int j = 0; j < neighbors.Length; j++)
                {
                    int neighbor = (int)neighbors[j];
                    long[] neighborsOfNeighbor = walkableGraph.GetPointConnections(neighbor);

                    for (int k = 0; k < neighborsOfNeighbor.Length; k++)
                    {
                        int candidate = (int)neighborsOfNeighbor[k];
                        if (candidate == i) continue;
                        if (walkableGraph.ArePointsConnected(i, candidate)) continue;

                        if (IsLineWalkable(posI, walkableGraph.GetPointPosition(candidate)))
                        {
                            walkableGraph.ConnectPoints(i, candidate);
                            changed = true;
                        }
                    }
                }
            }
        }
    }

    private bool IsLineWalkable(Vector2 from, Vector2 to)
    {
        int x0 = Mathf.FloorToInt(from.X + imageWidth / 2.0f);
        int y0 = Mathf.FloorToInt(from.Y + imageHeight / 2.0f);
        int x1 = Mathf.FloorToInt(to.X + imageWidth / 2.0f);
        int y1 = Mathf.FloorToInt(to.Y + imageHeight / 2.0f);

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
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

    private int FindNearestPointId(Vector2 position)
    {
        int pointCount = (int)walkableGraph.GetPointCount();
        if (pointCount == 0) return -1;

        int nearestId = 0;
        float closestDistanceSquared = position.DistanceSquaredTo(walkableGraph.GetPointPosition(0));

        for (int i = 1; i < pointCount; i++)
        {
            float distanceSquared = position.DistanceSquaredTo(walkableGraph.GetPointPosition(i));
            if (distanceSquared >= closestDistanceSquared) continue;

            closestDistanceSquared = distanceSquared;
            nearestId = i;
        }

        return nearestId;
    }

    public Vector2[] GetPathToRandomPoint(Vector2 fromPosition)
    {
        int pointCount = (int)walkableGraph.GetPointCount();
        if (pointCount < 2) return [];

        int startId = FindNearestPointId(fromPosition);

        int targetId = startId;
        while (targetId == startId)
            targetId = GD.RandRange(0, pointCount - 1);

        Vector2[] path = walkableGraph.GetPointPath(startId, targetId);
        if (path.Length <= 1) return [];

        return path;
    }

    public Vector2 GetRandomPointPosition()
    {
        int pointCount = (int)walkableGraph.GetPointCount();
        if (pointCount == 0) return Vector2.Zero;
        return walkableGraph.GetPointPosition(GD.RandRange(0, pointCount - 1));
    }

    private static bool IsWalkablePixel(Color color)
    {
        if (color.A < 0.1f) return false;
        float brightness = Mathf.Max(color.R, Mathf.Max(color.G, color.B));
        return brightness <= brightnessThreshold;
    }
}
