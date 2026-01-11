using Godot;

public partial class GrassGenerator : MultiMeshInstance3D
{
    [Export]
    public int InstanceCount = 20000;

    [Export]
    public Vector2 AreaSize = new Vector2(100, 100);

    [Export]
    public Color GrassColor = new Color(0.2f, 0.6f, 0.1f);

    public override void _Ready()
    {
        // 1. Create the Mesh for a single grass blade
        var mesh = new PrismMesh();
        mesh.Size = new Vector3(0.1f, 0.8f, 0.1f); // Thin and tall
        // Offset the mesh so the pivot is at the bottom (PrismMesh defaults to center)
        // Wait, PrismMesh doesn't have an easy offset property without a wrapper.
        // Let's stick to simple center pivot for now and offset the transform Y up.

        // Setup Material
        var material = new StandardMaterial3D();
        material.AlbedoColor = GrassColor;
        material.CullMode = BaseMaterial3D.CullModeEnum.Disabled; // Double sided
        material.ShadingMode = BaseMaterial3D.ShadingModeEnum.PerVertex; // Cheap shading
        mesh.Material = material;

        // 2. Setup MultiMesh
        var multiMesh = new MultiMesh();
        multiMesh.TransformFormat = MultiMesh.TransformFormatEnum.Transform3D;
        multiMesh.Mesh = mesh;
        multiMesh.InstanceCount = InstanceCount;

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
            var position = new Vector3(x, 0.4f * scale, z); // 0.4f puts base roughly at 0 since height is 0.8
            var basis = Basis.FromEuler(new Vector3(0, rotY, 0));
            basis = basis.Scaled(Vector3.One * scale);

            var transform = new Transform3D(basis, position);

            multiMesh.SetInstanceTransform(i, transform);
        }

        // Apply to Self
        this.Multimesh = multiMesh;
    }
}
