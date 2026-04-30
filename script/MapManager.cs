using Godot;
using System.Collections.Generic;

public partial class MapManager : Sprite2D
{
    [Export] private PackedScene mapMarkerScene;
    [Export] private NavigationArea navigationArea;
    [Export] private Camera2D camera;
    [Export] private SectionRevealer mapSectionToggle;
    [Export] private Vector2 trackingZoom = new(1.5f, 1.5f);

    private Dictionary<Human, MapMarker> humans = [];
    private MapMarker currentMarkerToTrack;
    private Tween cameraTween;
    private bool userControlling = false;
    private const float triangulationTime = 3f;

    private Vector2 _defaultMapPosition;

    public override void _Ready()
    {
        _defaultMapPosition = Position;
        SetProcessInput(true);

        camera.Position = GetViewportCenter();
        camera.Zoom = trackingZoom;
    }

    public override void _Input(InputEvent @event)
    {
        if (!mapSectionToggle.IsOpen) return;

        if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed)
        {
            if (mouseButton.ButtonIndex == MouseButton.WheelUp)
            {
                cameraTween?.Kill();
                Vector2 zoom = camera.Zoom;
                zoom = (zoom * 1.1f).Clamp(Vector2.One * 0.2f, Vector2.One * 5.0f);
                camera.Zoom = zoom;
                userControlling = true;
            }
            else if (mouseButton.ButtonIndex == MouseButton.WheelDown)
            {
                cameraTween?.Kill();
                Vector2 zoom = camera.Zoom;
                zoom = (zoom / 1.1f).Clamp(Vector2.One * 0.2f, Vector2.One * 5.0f);
                camera.Zoom = zoom;
                userControlling = true;
            }
        }

        if (@event is InputEventMouseMotion motion && Input.IsMouseButtonPressed(MouseButton.Left))
        {
            cameraTween?.Kill();
            camera.Position -= motion.Relative / camera.Zoom;
            userControlling = true;
        }
    }

    public override void _Process(double delta)
    {
        if (mapSectionToggle.IsOpen)
        {
            if (!userControlling)
                TrackHuman();
        }
        else
        {
            userControlling = false;
            camera.Zoom = trackingZoom;
            TrackHuman();
        }
    }

    private void TrackHuman()
    {
        if (currentMarkerToTrack == null) return;
        if (cameraTween != null && cameraTween.IsRunning()) return;
        camera.Position = MarkerWorldPosition(currentMarkerToTrack);
    }

    public void SetHumanToTrack(Human human)
    {
        currentMarkerToTrack?.SetSelected(false);
        if (!humans.TryGetValue(human, out MapMarker marker) || marker == null) return;

        currentMarkerToTrack = marker;
        currentMarkerToTrack.SetSelected(true);
        userControlling = false;

        Vector2 targetPos = MarkerWorldPosition(marker);

        cameraTween?.Kill();
        cameraTween = CreateTween().SetParallel(true);
        cameraTween.TweenProperty(camera, "position", targetPos, triangulationTime)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);
        cameraTween.TweenProperty(camera, "zoom", trackingZoom, triangulationTime)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);
    }

    public void RegisterHumanOnMap(Human human)
    {
        if (humans.ContainsKey(human)) return;
        MapMarker newMapMarker = mapMarkerScene.Instantiate<MapMarker>();
        navigationArea.AddChild(newMapMarker);
        newMapMarker.Position = navigationArea.HasPoints ? navigationArea.GetRandomPointPosition() : Vector2.Zero;
        newMapMarker.Human = human;
        newMapMarker.MovementFinished += () => human.SetMoving(false);
        humans.Add(human, newMapMarker);
    }

    public void SetHumanMarkerDestinationToRandomLocation(Human human)
    {
        if (human == null) return;
        if (!humans.TryGetValue(human, out MapMarker marker) || marker == null) return;
        if (!navigationArea.HasPoints || navigationArea.PointCount < 2) return;

        Vector2[] path = navigationArea.GetPathToRandomPoint(marker.Position);
        if (path.Length < 2) return;

        marker.SetPath(path);
        human.SetMoving(true);
    }

    private Vector2 MarkerWorldPosition(MapMarker marker)
    {
        return _defaultMapPosition + marker.Position * Scale;
    }

    private Vector2 GetViewportCenter()
    {
        Vector2I viewportSize = GetParent<SubViewport>().Size;
        return new(viewportSize.X / 2.0f, viewportSize.Y / 2.0f);
    }
}
