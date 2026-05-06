using Godot;
using System.Collections.Generic;

public partial class MapManager : Sprite2D
{
    [Export] private MapData mapData;
    [Export] private PackedScene mapMarkerScene;
    [Export] private Camera2D camera;
    [Export] private SectionRevealer mapSectionToggle;
    [Export] private float trackingZoom = 1.5f;
    private NavigationArea navigationArea = new();

    private readonly Dictionary<Human, MapMarker> humans = [];
    private MapMarker currentMarkerToTrack;
    private Tween zoomTween;
    private bool userControlling = false;
    private bool _isTriangulating = false;
    private ulong _trackingStartMsec;
    private Vector2 _trackingStartCameraPos;
    private const float triangulationTime = 3f;

    private Vector2 _defaultMapPosition;

    public override void _Ready()
    {
        AddChild(navigationArea);
        TextureFilter = TextureFilterEnum.NearestWithMipmapsAnisotropic;
        _defaultMapPosition = Position;
        SetProcessInput(true);

        Texture = mapData?.DisplayTexture;

        mapData?.LoadIntoGraph(navigationArea.WalkableGraph);

        SubViewport viewport = GetParent<SubViewport>();
        viewport.Size = new Vector2I(1920, 1080);

        if (viewport.GetParent() is Control container)
            container.SetAnchorsPreset(Control.LayoutPreset.FullRect);

        camera.Position = GetViewportCenter();
        camera.Zoom = new Vector2(trackingZoom, trackingZoom);
    }

    public override void _Input(InputEvent @event)
    {
        if (!mapSectionToggle.IsOpen) return;

        if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed)
        {
            if (mouseButton.ButtonIndex == MouseButton.WheelUp)
            {
                zoomTween?.Kill();
                _isTriangulating = false;
                Vector2 zoom = camera.Zoom;
                zoom = (zoom * 1.1f).Clamp(Vector2.One * 0.2f, Vector2.One * 5.0f);
                camera.Zoom = zoom;
                userControlling = true;
            }
            else if (mouseButton.ButtonIndex == MouseButton.WheelDown)
            {
                zoomTween?.Kill();
                _isTriangulating = false;
                Vector2 zoom = camera.Zoom;
                zoom = (zoom / 1.1f).Clamp(Vector2.One * 0.2f, Vector2.One * 5.0f);
                camera.Zoom = zoom;
                userControlling = true;
            }
        }

        if (@event is InputEventMouseMotion motion && Input.IsMouseButtonPressed(MouseButton.Left))
        {
            zoomTween?.Kill();
            _isTriangulating = false;
            camera.Position -= motion.Relative / camera.Zoom;
            userControlling = true;
        }
    }

    public override void _Process(double delta)
    {
        if (!mapSectionToggle.IsOpen)
            userControlling = false;

        if (!userControlling)
        {
            if (_isTriangulating)
                SmoothChaseMarker();
            else
                TrackHuman();
        }
    }

    private void TrackHuman()
    {
        if (currentMarkerToTrack == null) return;
        camera.Position = MarkerWorldPosition(currentMarkerToTrack);
    }

    private void SmoothChaseMarker()
    {
        if (currentMarkerToTrack == null) return;

        float elapsed = (Time.GetTicksMsec() - _trackingStartMsec) / 1000f;
        float t = Mathf.Clamp(elapsed / triangulationTime, 0f, 1f);
        float blend = 1f - (1f - t) * (1f - t) * (1f - t);
        Vector2 target = MarkerWorldPosition(currentMarkerToTrack);
        camera.Position = _trackingStartCameraPos.Lerp(target, blend);

        if (t >= 1f)
            _isTriangulating = false;
    }

    public void SetHumanToTrack(Human human)
    {
        currentMarkerToTrack?.SetSelected(false);
        if (!humans.TryGetValue(human, out MapMarker marker) || marker == null) return;

        currentMarkerToTrack = marker;
        currentMarkerToTrack.SetSelected(true);
        userControlling = false;
        _isTriangulating = true;
        _trackingStartMsec = Time.GetTicksMsec();
        _trackingStartCameraPos = camera.Position;

        zoomTween?.Kill();
        zoomTween = CreateTween();
        zoomTween.TweenProperty(camera, "zoom", new Vector2(trackingZoom,trackingZoom), triangulationTime)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);
    }

    public void RegisterHumanOnMap(Human human)
    {
        if (humans.ContainsKey(human)) return;
        MapMarker newMapMarker = mapMarkerScene.Instantiate<MapMarker>();
        navigationArea.AddChild(newMapMarker);
        newMapMarker.Position = navigationArea.GetRandomPointPosition();
        newMapMarker.Human = human;
        newMapMarker.MovementFinished += () => human.SetMoving(false);
        humans.Add(human, newMapMarker);
    }

    public void SetHumanMarkerDestinationToRandomLocation(Human human)
    {
        if (human == null) return;
        if (!humans.TryGetValue(human, out MapMarker marker) || marker == null) return;

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
