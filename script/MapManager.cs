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
        TrackHuman();
    }

    private void TrackHuman()
    {
        if(currentMarkerToTrack == null) return;
        if(currentMoveTween != null && currentMoveTween.IsRunning()) return;
        if(!currentMarkerToTrack.IsMoving) return;

        Position = GetViewportCenter() - (currentMarkerToTrack.Position * Scale);
    }

    public void SetHumanToTrack(Human human)
    {
        currentMarkerToTrack?.SetSelected(false);
        if(!humans.TryGetValue(human, out MapMarker marker) || marker == null) return;

        currentMarkerToTrack = marker;
        currentMarkerToTrack.SetSelected(true);
        Vector2 targetPosition = GetViewportCenter() - (marker.Position * Scale);

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
        newMapMarker.Position = navigationArea.HasPoints ? navigationArea.GetRandomPointPosition() : Vector2.Zero;
        newMapMarker.MovementFinished += () => human.SetMoving(false);
        humans.Add(human, newMapMarker);
    }

    public void SetHumanMarkerDestinationToRandomLocation(Human human)
    {
        if(human == null) return;
        if(!humans.TryGetValue(human, out MapMarker marker) || marker == null) return;
        if(!navigationArea.HasPoints || navigationArea.PointCount < 2) return;

        Vector2[] path = navigationArea.GetPathToRandomPoint(marker.Position);
        if(path.Length < 2) return;

        marker.SetPath(path);
        human.SetMoving(true);
    }
    private Vector2 GetViewportCenter()
    {
        Vector2I viewportSize = GetParent<SubViewport>().Size;
        return new(viewportSize.X / 2.0f, viewportSize.Y / 2.0f);
    }
}
