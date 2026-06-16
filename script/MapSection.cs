using System.Collections.Generic;
using Godot;

namespace DoomSlinger;

public partial class MapSection : Panel, ISection
{
    [Export] private SectionRevealer revealer;
    [Export] private MapData mapData;
    [Export] private PackedScene mapMarkerScene;
    [Export] private Camera2D camera;
    [Export] private Sprite2D mapSprite;
    [Export] private SubViewport viewport;
    [Export] private float trackingZoom = 1.5f;

    private NavigationArea navigationArea = new();
    private readonly Dictionary<Human, MapMarker> humans = [];
    private MapMarker currentMarkerToTrack;
    private Tween zoomTween;
    private bool userControlling;
    private bool _isTriangulating;
    private ulong _trackingStartMsec;
    private Vector2 _trackingStartCameraPos;
    private const float triangulationTime = 3f;

    private Vector2 _defaultMapPosition;
    private Vector2 _mapHalfSize;
    private Vector2 _minZoom;

    public void Toggle() => revealer?.Toggle();
    public void SetOpen(bool open) => revealer?.SetOpen(open);
    public bool IsOpen => revealer?.IsOpen ?? false;
    public Texture2D MapTexture => viewport?.GetTexture();

    public override void _Ready()
    {
        mapSprite.AddChild(navigationArea);
        mapSprite.TextureFilter = TextureFilterEnum.NearestWithMipmapsAnisotropic;
        _defaultMapPosition = mapSprite.Position;
        SetProcessInput(true);

        mapSprite.Texture = mapData?.DisplayTexture;
        mapData?.LoadIntoGraph(navigationArea.WalkableGraph);

        if (viewport.GetParent() is Control container)
            container.SetAnchorsPreset(LayoutPreset.FullRect);

        if (mapData?.DisplayTexture != null)
        {
            _mapHalfSize = mapData.DisplayTexture.GetSize() * mapSprite.Scale / 2;
            _minZoom = viewport.Size / (_mapHalfSize * 2);
        }

        camera.Position = GetViewportCenter();
        camera.Zoom = new Vector2(trackingZoom, trackingZoom);
    }

    public override void _Input(InputEvent @event)
    {
        if (!IsOpen) return;

        if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed)
        {
            if (mouseButton.ButtonIndex == MouseButton.WheelUp)
            {
                zoomTween?.Kill();
                _isTriangulating = false;
                Vector2 zoom = camera.Zoom;
                zoom = (zoom * 1.1f).Clamp(_minZoom, Vector2.One * 5.0f);
                camera.Zoom = zoom;
                ClampCameraToBounds();
                userControlling = true;
            }
            else if (mouseButton.ButtonIndex == MouseButton.WheelDown)
            {
                zoomTween?.Kill();
                _isTriangulating = false;
                Vector2 zoom = camera.Zoom;
                zoom = (zoom / 1.1f).Clamp(_minZoom, Vector2.One * 5.0f);
                camera.Zoom = zoom;
                ClampCameraToBounds();
                userControlling = true;
            }
        }

        if (@event is InputEventMouseMotion motion && Input.IsMouseButtonPressed(MouseButton.Left))
        {
            zoomTween?.Kill();
            _isTriangulating = false;
            camera.Position -= motion.Relative / camera.Zoom;
            ClampCameraToBounds();
            userControlling = true;
        }
    }

    public override void _Process(double delta)
    {
        if (!IsOpen)
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
        zoomTween.TweenProperty(camera, "zoom", new Vector2(trackingZoom, trackingZoom), triangulationTime)
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
        return _defaultMapPosition + marker.Position * mapSprite.Scale;
    }

    private Vector2 GetViewportCenter()
    {
        return new(viewport.Size.X / 2.0f, viewport.Size.Y / 2.0f);
    }

    private void ClampCameraToBounds()
    {
        if (_mapHalfSize == Vector2.Zero) return;
        Vector2 halfView = viewport.Size / (2 * camera.Zoom);
        Vector2 minPos = _defaultMapPosition - _mapHalfSize + halfView;
        Vector2 maxPos = _defaultMapPosition + _mapHalfSize - halfView;
        camera.Position = camera.Position.Clamp(minPos, maxPos);
    }
}
