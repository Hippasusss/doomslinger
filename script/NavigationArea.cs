using Godot;
using System.Collections.Generic;

public partial class NavigationArea : Node
{
    private List<Vector2> walkablePoints = [];
    private AStar2D walkableGraph = new();
    private Image sourceImage;
    private Vector2 halfImageSize;

    private const float brightnessThreshold = 0.35f;

    public bool HasPoints => walkablePoints.Count > 0;
    public int PointCount => walkablePoints.Count;

    public void BuildFromTexture(Texture2D texture)
    {
        if (texture == null) return;

        sourceImage = texture.GetImage();
        if (sourceImage == null || sourceImage.GetWidth() == 0 || sourceImage.GetHeight() == 0) return;

        walkablePoints.Clear();
        walkableGraph.Clear();

        Vector2I imageSize = new(sourceImage.GetWidth(), sourceImage.GetHeight());
        Dictionary<Vector2I, int> pointIds = [];
        halfImageSize = new(imageSize.X / 2.0f, imageSize.Y / 2.0f);
        int pointId = 0;

        for (int y = 0; y < imageSize.Y; y++)
        {
            for (int x = 0; x < imageSize.X; x++)
            {
                if (!IsWalkablePixel(sourceImage.GetPixel(x, y), brightnessThreshold)) continue;

                Vector2 position = new(x + 0.5f - halfImageSize.X, y + 0.5f - halfImageSize.Y);
                walkablePoints.Add(position);
                walkableGraph.AddPoint(pointId, position);
                pointIds.Add(new(x, y), pointId);
                pointId++;
            }
        }

        Vector2I[] neighborOffsets = [
            new(-1, -1), new(0, -1), new(1, -1),
            new(-1,  0),              new(1,  0),
            new(-1,  1), new(0,  1), new(1,  1)
        ];
        foreach (KeyValuePair<Vector2I, int> point in pointIds)
        {
            foreach (Vector2I offset in neighborOffsets)
            {
                Vector2I neighbor = point.Key + offset;
                if (!pointIds.TryGetValue(neighbor, out int neighborId)) continue;
                if (walkableGraph.ArePointsConnected(point.Value, neighborId)) continue;
                walkableGraph.ConnectPoints(point.Value, neighborId);
            }
        }

        PruneGraph();
        CompactGraph();
    }

    private void PruneGraph()
    {
        bool changed = true;
        while (changed)
        {
            changed = false;
            List<int> toRemove = [];

            foreach (int id in walkableGraph.GetPointIds())
            {
                long[] connections = walkableGraph.GetPointConnections(id);
                if (connections.Length != 2) continue;

                Vector2 posA = walkableGraph.GetPointPosition((int)connections[0]);
                Vector2 posB = walkableGraph.GetPointPosition((int)connections[1]);

                if (!IsLineWalkable(posA, posB)) continue;

                toRemove.Add(id);
            }

            if (toRemove.Count == 0) break;
            changed = true;

            foreach (int id in toRemove)
            {
                long[] connections = walkableGraph.GetPointConnections(id);
                if (connections.Length != 2) continue;

                int a = (int)connections[0];
                int b = (int)connections[1];

                walkableGraph.RemovePoint(id);
                if (!walkableGraph.ArePointsConnected(a, b))
                    walkableGraph.ConnectPoints(a, b);
            }
        }
    }

    private void CompactGraph()
    {
        long[] oldIds = walkableGraph.GetPointIds();
        if (oldIds.Length == 0) return;

        AStar2D compactedGraph = new();
        walkablePoints.Clear();
        Dictionary<long, int> idMap = [];

        for (int i = 0; i < oldIds.Length; i++)
        {
            Vector2 pos = walkableGraph.GetPointPosition(oldIds[i]);
            compactedGraph.AddPoint(i, pos);
            walkablePoints.Add(pos);
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
    }

    private bool IsLineWalkable(Vector2 from, Vector2 to)
    {
        float distance = from.DistanceTo(to);
        int steps = Mathf.Max(1, Mathf.CeilToInt(distance));

        for (int i = 0; i <= steps; i++)
        {
            float t = (float)i / steps;
            Vector2 samplePoint = new(
                Mathf.Lerp(from.X, to.X, t),
                Mathf.Lerp(from.Y, to.Y, t)
            );
            if (!IsWorldPositionWalkable(samplePoint))
                return false;
        }

        return true;
    }

    private bool IsWorldPositionWalkable(Vector2 worldPos)
    {
        int pixelX = Mathf.FloorToInt(worldPos.X + halfImageSize.X);
        int pixelY = Mathf.FloorToInt(worldPos.Y + halfImageSize.Y);

        if (pixelX < 0 || pixelX >= sourceImage.GetWidth() ||
            pixelY < 0 || pixelY >= sourceImage.GetHeight())
            return false;

        return IsWalkablePixel(sourceImage.GetPixel(pixelX, pixelY), brightnessThreshold);
    }

    public Vector2[] SimplifyPath(Vector2[] path)
    {
        if (path.Length <= 2) return path;

        List<Vector2> simplified = [path[0]];
        int currentIdx = 0;

        while (currentIdx < path.Length - 1)
        {
            int farthestReachable = currentIdx + 1;

            for (int testIdx = path.Length - 1; testIdx > currentIdx + 1; testIdx--)
            {
                if (IsLineWalkable(path[currentIdx], path[testIdx]))
                {
                    farthestReachable = testIdx;
                    break;
                }
            }

            simplified.Add(path[farthestReachable]);
            currentIdx = farthestReachable;
        }

        return simplified.ToArray();
    }

    public int FindNearestPointId(Vector2 position)
    {
        if (walkablePoints.Count == 0) return -1;

        int nearestId = 0;
        float closestDistanceSquared = position.DistanceSquaredTo(walkablePoints[0]);

        for (int i = 1; i < walkablePoints.Count; i++)
        {
            float distanceSquared = position.DistanceSquaredTo(walkablePoints[i]);
            if (distanceSquared >= closestDistanceSquared) continue;

            closestDistanceSquared = distanceSquared;
            nearestId = i;
        }

        return nearestId;
    }

    public Vector2[] GetPath(int fromId, int toId)
    {
        if (walkableGraph.GetPointCount() < 2) return [];
        return walkableGraph.GetPointPath(fromId, toId);
    }

    public int GetRandomPointIdExcluding(int excludeId)
    {
        if (walkablePoints.Count <= 1) return excludeId;

        int targetId = excludeId;
        while (targetId == excludeId)
        {
            targetId = GD.RandRange(0, walkablePoints.Count - 1);
        }

        return targetId;
    }

    public Vector2 GetRandomPointPosition()
    {
        if (walkablePoints.Count == 0) return Vector2.Zero;
        return walkablePoints[GD.RandRange(0, walkablePoints.Count - 1)];
    }

    private static bool IsWalkablePixel(Color color, float brightnessThreshold)
    {
        if (color.A < 0.1f) return false;
        float brightness = Mathf.Max(color.R, Mathf.Max(color.G, color.B));
        return brightness <= brightnessThreshold;
    }
}
