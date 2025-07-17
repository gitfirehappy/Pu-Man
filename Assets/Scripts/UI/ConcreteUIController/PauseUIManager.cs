using UnityEngine;

public class PauseUIManager : MonoBehaviour
{
    private bool isPaused = false;

    private void Awake()
    {
        EventBus.OnPauseUIRequested += ShowPauseUI;
        EventBus.OnResumeUIRequested += HidePauseUI;
    }

    private void OnDestroy()
    {
        EventBus.OnPauseUIRequested -= ShowPauseUI;
        EventBus.OnResumeUIRequested -= HidePauseUI;
    }

    private void Update()
    {
        // 检测ESC按键
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    private void TogglePause()
    {
        if (isPaused)
        {
            EventBus.TriggerGameResumed();
        }
        else
        {
            EventBus.TriggerGamePaused();
        }
        isPaused = !isPaused;
    }

    public void ShowPauseUI()
    {
        UIManager.Instance.ShowUIForm<PausePanel>();

        var pausePanel = UIManager.Instance.GetForm<PausePanel>();
        if (pausePanel != null)
        {
            UpdatePlayerStats(pausePanel);
            UpdateBuffCounts(pausePanel);
        }
    }

    public void HidePauseUI()
    {
        UIManager.Instance.HideUIForm<PausePanel>();
    }

    private void UpdatePlayerStats(PausePanel pausePanel)
    {
        var player = PlayerManager.Instance.Player;
        if (player == null) return;

        var health = player.Health;
        var shooting = player.Shooting;
        var movement = player.Movement;

        pausePanel.UpdatePlayerStats(
            health.CurrentHealth.ToString("F0"),
            health.Armor.ToString("F0"),
            health.HealthRegen.ToString("F1"),
            health.DodgeChance.ToString("P0"),
            health.CollisionDamage.ToString("F1"),
            shooting.Damage.ToString("F1"),
            shooting.FireRate.ToString("F1"),
            shooting.Knockback.ToString("F1"),
            shooting.ProjectileCount.ToString(),
            shooting.ProjectileSize.ToString("F1"),
            movement.RunSpeed.ToString("F1")
        );
    }

    private void UpdateBuffCounts(PausePanel pausePanel)
    {
        var buffManager = BuffManager.Instance;
        if (buffManager == null) return;

        pausePanel.UpdateBuffCounts(
            buffManager.GetBuffCount(Rarity.Common),
            buffManager.GetBuffCount(Rarity.Rare),
            buffManager.GetBuffCount(Rarity.Epic),
            buffManager.GetBuffCount(Rarity.Legendary)
        );
    }
}