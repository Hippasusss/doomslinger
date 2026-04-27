using Godot;
using System.Collections.Generic;

public partial class MapManager : Sprite2D
{
    [Export] private PackedScene mapMarkerScene;
    [Export] private NavigationRegion2D navRegion;
    [Export(PropertyHint.Range, "0,1,0.01")] private float walkableBrightnessThreshold = 0.35f;
    [Export(PropertyHint.Range, "0,12,1")] private int walkableExpansionPixels = 0;
    [Export(PropertyHint.Range, "0,10,0.1")] private float polygonSimplification = 0.75f;
    private Dictionary<Human, MapMarker> humans = [];
    private List<Vector2> walkablePoints = [];
    private AStar2D walkableGraph = new();
    private MapMarker currentMarkerToTrack;
    private Tween currentMoveTween;

    public override void _Ready()
    {
        GenerateNavigationFromMapTexture();
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
        newMapMarker.Position = GetRandomPointOnNavigationArea();
        newMapMarker.MovementFinished += () => human.SetMoving(false);
        humans.Add(human, newMapMarker);
    }

    public void MoveHumanMarkerToRandomLocation(Human human)
    {
        if(human == null) return;
        if(!humans.TryGetValue(human, out MapMarker marker) || marker == null) return;
        if(walkablePoints.Count < 2 || walkableGraph.GetPointCount() < 2) return;

        int startPointId = GetClosestWalkablePointId(marker.Position);
        if(startPointId < 0) return;

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

    private void GenerateNavigationFromMapTexture()
    {
        if(Texture == null || navRegion == null) return;

        Image image = Texture.GetImage();
        if(image == null || image.GetWidth() == 0 || image.GetHeight() == 0) return;

        Vector2I imageSize = new(image.GetWidth(), image.GetHeight());
        walkablePoints = BuildWalkablePoints(image, imageSize);
        BuildWalkableGraph(image, imageSize);
        bool[,] walkableMask = BuildExpandedWalkableMask(image, imageSize);
        NavigationPolygon navigationPolygon = new();
        BuildNavigationPolygonsFromMask(navigationPolygon, walkableMask, imageSize);
        navRegion.NavigationPolygon = navigationPolygon;
    }

    private List<Vector2> BuildWalkablePoints(Image image, Vector2I imageSize)
    {
        Vector2 halfImageSize = new(imageSize.X / 2.0f, imageSize.Y / 2.0f);
        List<Vector2> points = [];

        for(int y = 0; y < imageSize.Y; y++)
        {
            for(int x = 0; x < imageSize.X; x++)
            {
                if(!IsWalkablePixel(image.GetPixel(x, y))) continue;
                points.Add(new(x + 0.5f - halfImageSize.X, y + 0.5f - halfImageSize.Y));
            }
        }

        return points;
    }

    private void BuildWalkableGraph(Image image, Vector2I imageSize)
    {
        walkableGraph.Clear();
        Dictionary<Vector2I, int> pointIds = [];
        Vector2 halfImageSize = new(imageSize.X / 2.0f, imageSize.Y / 2.0f);
        int pointId = 0;

        for(int y = 0; y < imageSize.Y; y++)
        {
            for(int x = 0; x < imageSize.X; x++)
            {
                if(!IsWalkablePixel(image.GetPixel(x, y))) continue;

                Vector2 position = new(x + 0.5f - halfImageSize.X, y + 0.5f - halfImageSize.Y);
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

    private bool[,] BuildExpandedWalkableMask(Image image, Vector2I imageSize)
    {
        bool[,] mask = new bool[imageSize.X, imageSize.Y];
        int expansionRadius = Mathf.Max(0, walkableExpansionPixels);
        int expansionRadiusSquared = expansionRadius * expansionRadius;

        for(int y = 0; y < imageSize.Y; y++)
        {
            for(int x = 0; x < imageSize.X; x++)
            {
                if(!IsWalkablePixel(image.GetPixel(x, y))) continue;

                for(int offsetY = -expansionRadius; offsetY <= expansionRadius; offsetY++)
                {
                    int sampleY = y + offsetY;
                    if(sampleY < 0 || sampleY >= imageSize.Y) continue;

                    for(int offsetX = -expansionRadius; offsetX <= expansionRadius; offsetX++)
                    {
                        int sampleX = x + offsetX;
                        if(sampleX < 0 || sampleX >= imageSize.X) continue;
                        if((offsetX * offsetX) + (offsetY * offsetY) > expansionRadiusSquared) continue;

                        mask[sampleX, sampleY] = true;
                    }
                }
            }
        }

        return mask;
    }

    private bool IsWalkablePixel(Color color)
    {
        if(color.A < 0.1f) return false;
        float brightness = Mathf.Max(color.R, Mathf.Max(color.G, color.B));
        return brightness <= walkableBrightnessThreshold;
    }

    private void BuildNavigationPolygonsFromMask(NavigationPolygon navigationPolygon, bool[,] walkableMask, Vector2I imageSize)
    {
        Vector2 halfImageSize = new(imageSize.X / 2.0f, imageSize.Y / 2.0f);
        List<Vector2> vertices = [];
        List<int[]> polygons = [];
        Dictionary<Vector2I, int> vertexLookup = [];
        int width = imageSize.X;
        int height = imageSize.Y;

        for(int y = 0; y < height; y++)
        {
            int runStart = -1;

            for(int x = 0; x <= width; x++)
            {
                bool walkable = x < width && walkableMask[x, y];

                if(walkable && runStart == -1)
                {
                    runStart = x;
                    continue;
                }

                if(walkable || runStart == -1) continue;

                AddWalkableQuad(polygons, vertices, vertexLookup, halfImageSize, runStart, x, y);
                runStart = -1;
            }
        }

        navigationPolygon.SetVertices([.. vertices]);
        foreach(int[] polygon in polygons)
        {
            navigationPolygon.AddPolygon(polygon);
        }
    }

    private void AddWalkableQuad(List<int[]> polygons, List<Vector2> vertices, Dictionary<Vector2I, int> vertexLookup, Vector2 halfImageSize, int startX, int endX, int y)
    {
        int topLeft = GetOrAddVertex(vertices, vertexLookup, new(startX, y), halfImageSize);
        int topRight = GetOrAddVertex(vertices, vertexLookup, new(endX, y), halfImageSize);
        int bottomRight = GetOrAddVertex(vertices, vertexLookup, new(endX, y + 1), halfImageSize);
        int bottomLeft = GetOrAddVertex(vertices, vertexLookup, new(startX, y + 1), halfImageSize);

        polygons.Add([topLeft, topRight, bottomRight, bottomLeft]);
    }

    private int GetOrAddVertex(List<Vector2> vertices, Dictionary<Vector2I, int> vertexLookup, Vector2I gridPoint, Vector2 halfImageSize)
    {
        if(vertexLookup.TryGetValue(gridPoint, out int vertexIndex)) return vertexIndex;

        vertexIndex = vertices.Count;
        vertices.Add(new(gridPoint.X - halfImageSize.X, gridPoint.Y - halfImageSize.Y));
        vertexLookup.Add(gridPoint, vertexIndex);
        return vertexIndex;
    }

    private Vector2 GetRandomPointOnNavigationArea()
    {
        if(walkablePoints.Count > 0)
        {
            int randomIndex = GD.RandRange(0, walkablePoints.Count - 1);
            return walkablePoints[randomIndex];
        }

        NavigationPolygon navigationPolygon = navRegion.NavigationPolygon;
        if(navigationPolygon == null) return Vector2.Zero;

        Vector2[] vertices = navigationPolygon.GetVertices();
        int polygonCount = navigationPolygon.GetPolygonCount();
        if(vertices.Length == 0 || polygonCount == 0) return Vector2.Zero;

        List<(int[] polygon, float area)> weightedPolygons = [];
        float totalArea = 0.0f;

        for(int i = 0; i < polygonCount; i++)
        {
            int[] polygon = navigationPolygon.GetPolygon(i);
            if(polygon.Length < 3) continue;

            float area = GetPolygonArea(vertices, polygon);
            if(area <= 0.0f) continue;

            weightedPolygons.Add((polygon, area));
            totalArea += area;
        }

        if(totalArea <= 0.0f) return Vector2.Zero;

        float targetArea = GD.Randf() * totalArea;
        foreach((int[] polygon, float area) in weightedPolygons)
        {
            targetArea -= area;
            if(targetArea > 0.0f) continue;
            return GetRandomPointInPolygon(vertices, polygon);
        }

        return GetRandomPointInPolygon(vertices, weightedPolygons[^1].polygon);
    }

    private int GetClosestWalkablePointId(Vector2 localPosition)
    {
        if(walkablePoints.Count == 0) return -1;

        int closestPointId = 0;
        float closestDistanceSquared = localPosition.DistanceSquaredTo(walkablePoints[0]);
        for(int i = 1; i < walkablePoints.Count; i++)
        {
            float distanceSquared = localPosition.DistanceSquaredTo(walkablePoints[i]);
            if(distanceSquared >= closestDistanceSquared) continue;

            closestDistanceSquared = distanceSquared;
            closestPointId = i;
        }

        return closestPointId;
    }

    private float GetPolygonArea(Vector2[] vertices, int[] polygon)
    {
        float doubledArea = 0.0f;

        for(int i = 0; i < polygon.Length; i++)
        {
            Vector2 current = vertices[polygon[i]];
            Vector2 next = vertices[polygon[(i + 1) % polygon.Length]];
            doubledArea += (current.X * next.Y) - (next.X * current.Y);
        }

        return Mathf.Abs(doubledArea) * 0.5f;
    }

    private Vector2 GetRandomPointInPolygon(Vector2[] vertices, int[] polygon)
    {
        if(polygon.Length == 3)
        {
            return GetRandomPointInTriangle(vertices[polygon[0]], vertices[polygon[1]], vertices[polygon[2]]);
        }

        List<(Vector2 a, Vector2 b, Vector2 c, float area)> triangles = [];
        float totalArea = 0.0f;
        Vector2 origin = vertices[polygon[0]];

        for(int i = 1; i < polygon.Length - 1; i++)
        {
            Vector2 a = origin;
            Vector2 b = vertices[polygon[i]];
            Vector2 c = vertices[polygon[i + 1]];
            float area = Mathf.Abs((b - a).Cross(c - a)) * 0.5f;
            if(area <= 0.0f) continue;

            triangles.Add((a, b, c, area));
            totalArea += area;
        }

        if(totalArea <= 0.0f) return origin;

        float targetArea = GD.Randf() * totalArea;
        foreach((Vector2 a, Vector2 b, Vector2 c, float area) in triangles)
        {
            targetArea -= area;
            if(targetArea > 0.0f) continue;
            return GetRandomPointInTriangle(a, b, c);
        }

        var lastTriangle = triangles[^1];
        return GetRandomPointInTriangle(lastTriangle.a, lastTriangle.b, lastTriangle.c);
    }

    private Vector2 GetRandomPointInTriangle(Vector2 a, Vector2 b, Vector2 c)
    {
        float r1 = Mathf.Sqrt(GD.Randf());
        float r2 = GD.Randf();

        return ((1.0f - r1) * a)
            + (r1 * (1.0f - r2) * b)
            + (r1 * r2 * c);
    }

}
