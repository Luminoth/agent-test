

public partial class IsometricCharacterController : CharacterBody3D
{
    [Export]
    public float Speed = 5.0f;
    [Export]
    public float JumpVelocity = 4.5f;
    [Export]
    public float DashSpeed = 15.0f;
    [Export]
    public float DashDuration = 0.2f;
    [Export]
    public float DashCooldown = 1.0f;

    // Get the gravity from the project settings to be synced with RigidBody nodes.
    public float Gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    private bool _isDashing = false;
    private float _dashTimer = 0.0f;
    private float _dashCooldownTimer = 0.0f;

    public override void _PhysicsProcess(double delta)
    {
        Vector3 velocity = Velocity;

        if (_dashCooldownTimer > 0)
            _dashCooldownTimer -= (float)delta;

        // Handle Dash Timer
        if (_isDashing)
        {
            _dashTimer -= (float)delta;
            if (_dashTimer <= 0)
            {
                _isDashing = false;
                velocity.X = 0; // Optional: Stop momentarily after dash
                velocity.Z = 0;
            }
            else
            {
                 // Continue dashing in current velocity direction
                 Velocity = velocity;
                 MoveAndSlide();
                 return; // Skip normal movement logic
            }
        }

        // Add the gravity.
        if (!IsOnFloor())
            velocity.Y -= Gravity * (float)delta;

        // Handle Jump (Original Logic Preserved)
        if (Input.IsActionJustPressed("jump") && IsOnFloor())
            velocity.Y = JumpVelocity;

        // Handle Dash Input
        if (Input.IsActionJustPressed("dash") && IsOnFloor() && !_isDashing && _dashCooldownTimer <= 0)
        {
             _isDashing = true;
             _dashTimer = DashDuration;
             _dashCooldownTimer = DashCooldown;

             // Dash in current facing direction or input direction
             Vector2 dashInputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_back");
             Vector3 dashDir;

             if (dashInputDir != Vector2.Zero)
             {
                 dashDir = new Vector3(dashInputDir.X, 0, dashInputDir.Y).Rotated(Vector3.Up, Mathf.DegToRad(45)).Normalized();
             }
             else
             {
                 // Dash forward if no input
                 // Assuming model faces -Z or +Z, but let's use current velocity or default forward
                 dashDir = -Transform.Basis.Z;
             }

             velocity = dashDir * DashSpeed;
             // Keep slight upward velocity or set to 0? Set to 0 for ground dash.
             velocity.Y = 0;

             Velocity = velocity;
             MoveAndSlide();
             return;
        }

        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_back");

        // Transform input to isometric direction (45 degrees)
        // We assume the camera is looking from a standard isometric angle
        Vector3 direction = new Vector3(inputDir.X, 0, inputDir.Y).Rotated(Vector3.Up, Mathf.DegToRad(45));
        direction = direction.Normalized();

        if (direction != Vector3.Zero)
        {
            velocity.X = direction.X * Speed;
            velocity.Z = direction.Z * Speed;

            // Optional: Rotate character to face movement direction
            LookAt(GlobalPosition + direction, Vector3.Up);
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
            velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
        }

        Velocity = velocity;
        MoveAndSlide();
    }
}
