

public partial class TestIsometricMovement : Node
{
    [Export]
    public IsometricCharacterController Character;
    private double _timer = 0;

    public override void _Ready()
    {
        if (Character == null)
        {
            GD.PrintErr("TestIsometricMovement: Character export variable must be assigned!");
            SetPhysicsProcess(false);
            return;
        }

        GD.Print("TestIsometricMovement: Linked to controller. Ready for manual verification.");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Character != null && Character.Velocity.LengthSquared() > 0.1f)
        {
            GD.Print($"Moving: Pos={Character.GlobalPosition}, Vel={Character.Velocity}");
        }
    }
}
