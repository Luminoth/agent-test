

public partial class DashHUD : Control
{
    [Export]
    public TextureProgressBar AbilityIcon;

    private IsometricCharacterController _player;
    private bool _wasOnCooldown = false;

    public void ConnectPlayer(IsometricCharacterController player)
    {
        _player = player;
    }

    public override void _Process(double delta)
    {
        if (_player == null || AbilityIcon == null) return;

        bool isOnCooldown = !_player.DashCooldownTimer.IsStopped();

        if (isOnCooldown)
        {
            // Cooldown Progress
            double timeLeft = _player.DashCooldownTimer.TimeLeft;
            double totalTime = _player.DashCooldown;

            // value goes from 0 to 100 as it recharges
            double rechargeProgress = (1.0 - (timeLeft / totalTime)) * 100.0;
            AbilityIcon.Value = rechargeProgress;

            // AbilityIcon.Modulate = Colors.Gray;
            _wasOnCooldown = true;
        }
        else
        {
            AbilityIcon.Value = 100;

            if (_wasOnCooldown)
            {
                // Transition to Ready: Flash Green
                FlashReady();
                _wasOnCooldown = false;
            }
            else
            {
                // Ready State: White (Normal)
                AbilityIcon.Modulate = AbilityIcon.Modulate.Lerp(Colors.White, (float)delta * 2.0f);
            }
        }
    }

    private void FlashReady()
    {
        AbilityIcon.Modulate = Colors.Green;
    }
}
