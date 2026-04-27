using Godot;
using System.Collections.Generic;

public partial class MapManager : Sprite2D
{
    [Export] private PackedScene mapMarkerScene;
    [Export] private NavigationRegion2D navRegion;
    [Export(PropertyHint.Range, "0,1,0.01")] private float walkableBrightnessThreshold = 0.35f;
    private Dictionary<Human, MapMarker> humans = [];
    private List<Vector2> walkablePoints = [];
    private AStar2D walkableGraph = new();
    private MapMarker currentMarkerToTrack;
    private Tween currentMoveTween;

    public override void _Ready()
    {
        BuildWalkableGraph();
    }

    public override void _Process(double delta)
    {
        if(currentMarkerToTrack == null) return;
        if(currentMoveTween != null && currentMoveTween.IsRunning()) return;
        if(!currentMarkerToTrack.IsMoving) return;

        Vector2I viewportSize = GetParent<SubViewport>().Size;
        Vector2 viewportCenter = new(viewportSize.X / 2.0f, viewportSize.Y / 2.0f);
        Vector2 targetPosition = viewportCenter - (currentMarkerToTrack.Position * Scale);

        Position = targetPosition;
    }

    public void TrackHuman(Human human)
    {
        if(currentMarkerToTrack != null) currentMarkerToTrack.SetSelected(false);
        if(!humans.TryGetValue(human, out MapMarker marker) || marker == null) return;

        currentMarkerToTrack = marker;
        currentMarkerToTrack.SetSelected(true);
        Vector2I viewportSize = GetParent<SubViewport>().Size;
        Vector2 viewportCenter = new(viewportSize.X / 2.0f, viewportSize.Y / 2.0f);
        Vector2 targetPosition = viewportCenter - (marker.Position * Scale);

        currentMoveTween?.Kill();
        currentMoveTween = CreateTween();
        currentMoveTween.TweenProperty(this, "position", targetPosition, 0.25f)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);
    }

    public void RegisterHumanOnMap(Human human)
    {
        if(humans.ContainsKey(human)) return;
        MapMarker newMapMarker = mapMarkerScene.Instantiate<MapMarker>();
        navRegion.AddChild(newMapMarker);
        newMapMarker.Position = walkablePoints.Count == 0
            ? Vector2.Zero
            : walkablePoints[GD.RandRange(0, walkablePoints.Count - 1)];
        newMapMarker.MovementFinished += () => human.SetMoving(false);
        humans.Add(human, newMapMarker);
    }

    public void MoveHumanMarkerToRandomLocation(Human human)
    {
        if(human == null) return;
        if(!humans.TryGetValue(human, out MapMarker marker) || marker == null) return;
        if(walkablePoints.Count < 2 || walkableGraph.GetPointCount() < 2) return;

        int startPointId = 0;
        float closestDistanceSquared = marker.Position.DistanceSquaredTo(walkablePoints[0]);
        for(int i = 1; i < walkablePoints.Count; i++)
        {
            float distanceSquared = marker.Position.DistanceSquaredTo(walkablePoints[i]);
            if(distanceSquared >= closestDistanceSquared) continue;

            closestDistanceSquared = distanceSquared;
            startPointId = i;
        }

        int targetPointId = startPointId;
        while(targetPointId == startPointId)
        {
            targetPointId = GD.RandRange(0, walkablePoints.Count - 1);
        }

        Vector2[] path = walkableGraph.GetPointPath(startPointId, targetPointId);
        if(path.Length <= 1) return;

        marker.SetPath(path);
        human.SetMoving(true);
    }

    private void BuildWalkableGraph()
    {
        if(Texture == null || navRegion == null) return;

        Image image = Texture.GetImage();
        if(image == null || image.GetWidth() == 0 || image.GetHeight() == 0) return;

        walkablePoints.Clear();
        walkableGraph.Clear();

        Vector2I imageSize = new(image.GetWidth(), image.GetHeight());
        Dictionary<Vector2I, int> pointIds = [];
        Vector2 halfImageSize = new(imageSize.X / 2.0f, imageSize.Y / 2.0f);
        int pointId = 0;

        for(int y = 0; y < imageSize.Y; y++)
        {
            for(int x = 0; x < imageSize.X; x++)
            {
                if(!IsWalkablePixel(image.GetPixel(x, y))) continue;

                Vector2 position = new(x + 0.5f - halfImageSize.X, y + 0.5f - halfImageSize.Y);
                walkablePoints.Add(position);
                walkableGraph.AddPoint(pointId, position);
                pointIds.Add(new(x, y), pointId);
                pointId++;
            }
        }

        Vector2I[] neighborOffsets = [new(0, -1), new(-1, 0), new(1, 0), new(0, 1)];
        foreach(KeyValuePair<Vector2I, int> point in pointIds)
        {
            foreach(Vector2I offset in neighborOffsets)
            {
                Vector2I neighbor = point.Key + offset;
                if(!pointIds.TryGetValue(neighbor, out int neighborId)) continue;
                if(walkableGraph.ArePointsConnected(point.Value, neighborId)) continue;
                walkableGraph.ConnectPoints(point.Value, neighborId);
            }
        }
    }

    private bool IsWalkablePixel(Color color)
    {
        if(color.A < 0.1f) return false;
        float brightness = Mathf.Max(color.R, Mathf.Max(color.G, color.B));
        return brightness <= walkableBrightnessThreshold;
    }
}
