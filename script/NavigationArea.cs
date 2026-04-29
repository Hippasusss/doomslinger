using Godot;

public partial class NavigationArea : Node
{
    private AStar2D walkableGraph = new();
    [Export] public NavigationGraphData NavigationGraphData { get; set; }

    public bool HasPoints => walkableGraph.GetPointCount() > 0;
    public int PointCount => (int)walkableGraph.GetPointCount();

    public override void _Ready()
    {
        NavigationGraphData?.LoadIntoGraph(walkableGraph);
    }

    public Vector2[] GetPathToRandomPoint(Vector2 fromPosition)
    {
        int pointCount = (int)walkableGraph.GetPointCount();
        if (pointCount < 2) return [];

        int startId = 0;
        float closestDistanceSquared = fromPosition.DistanceSquaredTo(walkableGraph.GetPointPosition(0));
        for (int i = 1; i < pointCount; i++)
        {
            float distanceSquared = fromPosition.DistanceSquaredTo(walkableGraph.GetPointPosition(i));
            if (distanceSquared >= closestDistanceSquared) continue;
            closestDistanceSquared = distanceSquared;
            startId = i;
        }

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
}
