using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : SingletonMono<AudioManager>
{

    [Header("配置")]
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private SceneBGMConfigSO bgmConfig;
    [SerializeField] private AudioClip defaultBGM; // 默认背景音乐

    [Header("音源")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    private GameState currentState;
    private int currentSceneIndex;

    protected override void Init()
    {
        // 初始化音量服务
        AudioVolumeService.Init(mixer);

        // 自动创建 AudioSource
        bgmSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();

        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        sfxSource.playOnAwake = false;

        // 自动绑定 AudioMixerGroup
        var musicGroup = mixer.FindMatchingGroups("Music");
        if (musicGroup.Length > 0)
            bgmSource.outputAudioMixerGroup = musicGroup[0];

        var sfxGroup = mixer.FindMatchingGroups("SFX");
        if (sfxGroup.Length > 0)
            sfxSource.outputAudioMixerGroup = sfxGroup[0];

        SceneManager.sceneLoaded += OnSceneLoaded;
        EventBus.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        EventBus.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentSceneIndex = scene.buildIndex;
        UpdateBGM();
    }

    private void OnGameStateChanged(GameState newState)
    {
        currentState = newState;
        UpdateBGM();
    }

    public void UpdateBGM()
    {
        string condition = null;

        // 检查是否是Boss战
        if (currentState == GameState.Battle &&
            EnemyManager.Instance.GetActiveBoss() != null)
        {
            condition = "BossBattle";
        }

        AudioClip newBGM = bgmConfig?.GetBGMForSceneAndState(
            currentSceneIndex,
            currentState,
            condition) ?? defaultBGM;

        if (newBGM == null)
        {
            bgmSource.Stop();
            return;
        }

        // 如果音乐不同才切换
        if (bgmSource.clip != newBGM)
        {
            bgmSource.clip = newBGM;
            bgmSource.Play();
        }
    }


    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip);
    }

    // 添加淡入淡出功能(可选)
    public IEnumerator FadeOutBGM(float duration)
    {
        float startVolume = bgmSource.volume;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(startVolume, 0, t / duration);
            yield return null;
        }
        bgmSource.Stop();
        bgmSource.volume = startVolume;
    }

    public IEnumerator FadeInBGM(float duration)
    {
        float startVolume = bgmSource.volume;
        bgmSource.volume = 0;
        bgmSource.Play();
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(0, startVolume, t / duration);
            yield return null;
        }
    }
}
