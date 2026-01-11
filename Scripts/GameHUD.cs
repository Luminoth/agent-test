

public partial class GameHUD : CanvasLayer
{
    [Export]
    public DashHUD DashHUDElement;

    public void ConnectPlayer(IsometricCharacterController player)
    {
        if (DashHUDElement != null)
        {
            DashHUDElement.ConnectPlayer(player);
        }
    }
}
