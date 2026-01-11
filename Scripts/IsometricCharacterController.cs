

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
    public float DashCooldown = 3.0f;

    [Export]
    public PackedScene DashVFX;

    // Get the gravity from the project settings to be synced with RigidBody nodes.
    public float Gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    private bool _isDashing = false;

    [Export]
    public Timer DashTimer;
    [Export]
    public Timer DashCooldownTimer;

    [Export]
    public AudioStreamPlayer3D DashSoundPlayer;

    public override void _Ready()
    {
        if (DashTimer != null) DashTimer.WaitTime = DashDuration;
        if (DashCooldownTimer != null) DashCooldownTimer.WaitTime = DashCooldown;

        // Connect to HUD
        // We find the first node in the "HUD" group and interact with it.
        SceneTree tree = GetTree();
        if (tree != null)
        {
            // Connect to HUD
            var hudNodes = tree.GetNodesInGroup("HUD");
            if (hudNodes.Count > 0)
            {
                var hud = hudNodes[0] as GameHUD;
                if (hud != null)
                {
                    hud.ConnectPlayer(this);
                }
            }

            // Connect to Ring
            var ringNodes = tree.GetNodesInGroup("Ring");
            if (ringNodes.Count > 0)
            {
                var ring = ringNodes[0] as Ring;
                if (ring != null)
                {
                    ring.RegisterPlayer(this);
                }
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector3 velocity = Velocity;

        // Handle Dash Logic
        if (_isDashing)
        {
            if (DashTimer.IsStopped())
            {
                // Dash just finished
                _isDashing = false;
                velocity.X = 0; // Optional: Stop momentarily after dash
                velocity.Z = 0;

                // Start Cooldown exactly when dash ends
                DashCooldownTimer.Start();
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
        /*if (Input.IsActionJustPressed("jump") && IsOnFloor())
            velocity.Y = JumpVelocity;*/

        // Handle Dash Input
        if (Input.IsActionJustPressed("dash") && IsOnFloor() && !_isDashing && DashCooldownTimer.IsStopped())
        {
             _isDashing = true;
             // Ensure WaitTimes are up to date if changed in runtime (optional)
             if (DashTimer != null) DashTimer.WaitTime = DashDuration;
             if (DashCooldownTimer != null) DashCooldownTimer.WaitTime = DashCooldown;

             if (DashTimer != null) DashTimer.Start();

             // Spawn VFX
             if (DashVFX != null)
             {
                 var vfx = DashVFX.Instantiate<Node3D>();
                 vfx.Position = Position + Vector3.Down * 0.9f;
                 GetParent().AddChild(vfx);
             }

             if (DashSoundPlayer != null)
             {
                 DashSoundPlayer.Play();
             }

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
