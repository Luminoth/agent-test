using Godot;

public partial class GrassGenerator : MultiMeshInstance3D
{
    [Export]
    public int InstanceCount = 20000;

    [Export]
    public Vector2 AreaSize = new Vector2(100, 100);

    [Export]
    public Mesh BladeMesh;

    [Export]
    public Material BladeMaterial;

    public override void _Ready()
    {
        if (BladeMesh == null)
        {
            GD.PrintErr("GrassGenerator: BladeMesh is missing!");
            return;
        }

        // 2. Setup MultiMesh
        var multiMesh = new MultiMesh();
        multiMesh.TransformFormat = MultiMesh.TransformFormatEnum.Transform3D;
        multiMesh.Mesh = BladeMesh;
        multiMesh.InstanceCount = InstanceCount;

        // Use Global Material Override if set (more efficient for instancing)
        if (BladeMaterial != null)
        {
            this.MaterialOverride = BladeMaterial;
        }

        // 3. Populate Instances
        var rng = new RandomNumberGenerator();
        rng.Randomize();

        for (int i = 0; i < InstanceCount; i++)
        {
            // Random Position
            float x = rng.RandfRange(-AreaSize.X / 2, AreaSize.X / 2);
            float z = rng.RandfRange(-AreaSize.Y / 2, AreaSize.Y / 2);

            // Random Rotation (Y-axis only)
            float rotY = rng.RandfRange(0, Mathf.Tau);

            // Random Scale variance
            float scale = rng.RandfRange(0.7f, 1.3f);

            // Construct Transform
            // NOTE: BladeMesh is centered. We lift it by half its height approx if using PrismMesh.
            // But allowing the mesh to handle its own origin is better.
            // For the default PrismMesh (which is centered), we lift by 0.4.
            float yOffset = 0.4f;

            var position = new Vector3(x, yOffset * scale, z);
            var basis = Basis.FromEuler(new Vector3(0, rotY, 0));
            basis = basis.Scaled(Vector3.One * scale);

            var transform = new Transform3D(basis, position);

            multiMesh.SetInstanceTransform(i, transform);
        }

        // Apply to Self
        this.Multimesh = multiMesh;
    }
}
