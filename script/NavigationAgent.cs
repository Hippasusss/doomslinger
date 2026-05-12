using Godot;

namespace DoomSlinger;

public partial class NavigationAgent : Node
{
    [Signal] public delegate void MovementFinishedEventHandler();

    [Export(PropertyHint.Range, "1,300,1")] private float moveSpeed = 10.0f;
    [Export(PropertyHint.Range, "0.01,10,0.01")] private float pointArrivalThreshold = 0.05f;

    private Vector2[] currentPath = [];
    private int currentPathIndex = 0;

    public bool IsMoving => currentPathIndex < currentPath.Length;
    public float MoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = value;
    }

    public void SetPath(Vector2[] path, Vector2 currentPosition)
    {
        currentPath = path;
        currentPathIndex = 0;

        while (currentPathIndex < currentPath.Length && currentPosition.DistanceTo(currentPath[currentPathIndex]) <= pointArrivalThreshold)
        {
            currentPathIndex++;
        }

        if (currentPathIndex < currentPath.Length) return;

        currentPath = [];
        currentPathIndex = 0;
        EmitSignal(SignalName.MovementFinished);
    }

    public void ProcessMovement(Node2D body, double delta)
    {
        if (!IsMoving) return;

        Vector2 nextPathPosition = currentPath[currentPathIndex];
        body.Position = body.Position.MoveToward(nextPathPosition, moveSpeed * (float)delta);

        if (!body.Position.IsEqualApprox(nextPathPosition) && body.Position.DistanceTo(nextPathPosition) > pointArrivalThreshold) return;

        body.Position = nextPathPosition;
        currentPathIndex++;
        while (currentPathIndex < currentPath.Length && body.Position.DistanceTo(currentPath[currentPathIndex]) <= pointArrivalThreshold)
        {
            currentPathIndex++;
        }

        if (currentPathIndex < currentPath.Length) return;

        currentPath = [];
        currentPathIndex = 0;
        EmitSignal(SignalName.MovementFinished);
    }
}
