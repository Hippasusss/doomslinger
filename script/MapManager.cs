using Godot;
using System.Collections.Generic;

public partial class MapManager : Sprite2D
{
    [Export] private PackedScene mapMarkerScene;
    private NavigationArea navigationArea = new();
    private Dictionary<Human, MapMarker> humans = [];
    private MapMarker currentMarkerToTrack;
    private Tween currentMoveTween;

    public override void _Ready()
    {
        navigationArea.BuildFromTexture(Texture);
        AddChild(navigationArea);
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
        AddChild(newMapMarker);
        newMapMarker.Position = navigationArea.HasPoints
            ? navigationArea.GetRandomPointPosition()
            : Vector2.Zero;
        newMapMarker.MovementFinished += () => human.SetMoving(false);
        humans.Add(human, newMapMarker);
    }

    public void MoveHumanMarkerToRandomLocation(Human human)
    {
        if(human == null) return;
        if(!humans.TryGetValue(human, out MapMarker marker) || marker == null) return;
        if(!navigationArea.HasPoints || navigationArea.PointCount < 2) return;

        int startPointId = navigationArea.FindNearestPointId(marker.Position);
        int targetPointId = navigationArea.GetRandomPointIdExcluding(startPointId);

        Vector2[] rawPath = navigationArea.GetPath(startPointId, targetPointId);
        if(rawPath.Length <= 1) return;

        marker.SetPath(navigationArea.SimplifyPath(rawPath));
        human.SetMoving(true);
    }


}
