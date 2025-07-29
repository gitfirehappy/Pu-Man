using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : UIFormBase
{
    [SerializeField][Header("通过波次")] private TextMeshProUGUI throughWavesText;
    [SerializeField][Header("返回菜单按钮")] private Button backButton;

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
            int wavesPassed = WaveCounter.Instance.CurrentWave - 1;
            throughWavesText.text = $"通过波次: {wavesPassed}";
        }
    }
}
