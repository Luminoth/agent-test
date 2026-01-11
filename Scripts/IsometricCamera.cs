

public partial class IsometricCamera : Camera3D
{
    [Export]
    public Node3D Target;

    [Export]
    public float SmoothSpeed = 5.0f;

    private Vector3 _offset;

    public override void _Ready()
    {
        if (Target == null)
        {
            GD.PrintErr("IsometricCamera: Target is not assigned!");
            SetPhysicsProcess(false);
            return;
        }

        // To center the target, we position the camera 'Distance' units away along its own backward vector
        float distance = GlobalPosition.DistanceTo(Target.GlobalPosition);
        GlobalPosition = Target.GlobalPosition + GlobalTransform.Basis.Z * distance;

        // Calculate initial offset based on this new centered position
        _offset = GlobalPosition - Target.GlobalPosition;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Target == null) return;

        Vector3 targetPos = Target.GlobalPosition + _offset;

        // Smoothly interpolate current position to target position
        Vector3 newPos = GlobalPosition.Lerp(targetPos, (float)delta * SmoothSpeed);

        GlobalPosition = newPos;
    }
}
