public partial class VFX_AutoDestroy : CpuParticles3D
{
    public override void _Ready()
    {
        Finished += QueueFree;
    }
}
