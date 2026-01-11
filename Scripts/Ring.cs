using Godot;

public partial class Ring : Node3D
{
    [Export]
    public float Radius = 25.0f;

    [Export]
    public ColorRect DamageOverlay;

    private Node3D _player;

    public override void _Ready()
    {
        AddToGroup("Ring");
    }

    public void RegisterPlayer(Node3D player)
    {
        _player = player;
    }

    public override void _Process(double delta)
    {
        if (_player == null || DamageOverlay == null) return;

        // Calculate horizontal distance
        Vector2 playerPos = new Vector2(_player.GlobalPosition.X, _player.GlobalPosition.Z);
        Vector2 ringPos = new Vector2(GlobalPosition.X, GlobalPosition.Z);
        float distance = playerPos.DistanceTo(ringPos);

        float targetAlpha = 0.0f;

        if (distance > Radius)
        {
            // Outside the ring
            targetAlpha = 0.5f;
            // Optional: modulate intensity based on how far out?
            // For now, simple fade to red
        }

        // Smoothly lerp overlay alpha
        Color c = DamageOverlay.Color;
        c.A = Mathf.Lerp(c.A, targetAlpha, (float)delta * 5.0f);
        DamageOverlay.Color = c;
    }
}
