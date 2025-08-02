using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : UIFormBase
{
    [Header("通过波次")] public TextMeshProUGUI throughWavesText;
    [Header("返回菜单按钮")] public Button backButton;

    protected override void Init()
    {
        gameObject.AddComponent<ButtonSoundInitializer>();

        backButton.onClick.AddListener(OnBackToMenu);
        UpdateWaveCount();
    }

    private void OnBackToMenu()
    {
        EventBus.TriggerChangeState(GameState.Menu);
    }

    private void UpdateWaveCount()
    {
        if (throughWavesText != null)
        {
            // 获取当前波次(注意从几开始，可能需要调整)
            var currentWave = WaveCounter.Instance.CurrentWave;
            int wavesPassed = currentWave - 1;
            if(WaveCounter.Instance.TotalWaves == currentWave && !WaveCounter.Instance.IsInEndlessMode)
                wavesPassed++;
            throughWavesText.text = $"通过波次: {wavesPassed}";

            // 保存记录
            DataManager.Instance.UpdateRecord(
                PlayerManager.Instance.LastPlayerType,
                wavesPassed
            );
        }
    }
}
