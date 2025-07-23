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
    [SerializeField] private int maxConcurrentSFX = 5; // 最大并发SFX数量

    private GameState currentState;
    private int currentSceneIndex;
    private List<AudioSource> sfxSources = new List<AudioSource>(); // SFX音源池
    private string currentConditionTag = null; // 当前条件标签

    protected override void Init()
    {
        // 空引用保护
        if (mixer == null) Debug.LogWarning("AudioMixer 未分配！");
        if (bgmConfig == null) Debug.LogWarning("BGM配置 未分配！");

        // 初始化音量服务
        AudioVolumeService.Init(mixer);

        // 创建SFX音源池
        for (int i = 0; i < maxConcurrentSFX; i++)
        {
            CreateSFXSource();
        }

        // BGM音源初始化
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        var musicGroup = mixer.FindMatchingGroups("Music")[0];
        bgmSource.outputAudioMixerGroup = musicGroup;

        SceneManager.sceneLoaded += OnSceneLoaded;
        EventBus.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        EventBus.OnGameStateChanged -= OnGameStateChanged;
    }

    // 创建新的SFX音源
    private AudioSource CreateSFXSource()
    {
        var source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];
        sfxSources.Add(source);
        return source;
    }

    // 获取空闲的SFX音源
    private AudioSource GetAvailableSFXSource()
    {
        foreach (var source in sfxSources)
        {
            if (!source.isPlaying) return source;
        }
        return CreateSFXSource(); // 没有空闲时创建新音源
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

    /// <summary>
    /// 更新BGM
    /// </summary>
    /// <param name="conditionTag">音乐标签</param>
    public void UpdateBGM(string conditionTag = null)
    {
        currentConditionTag = conditionTag;
        AudioClip newBGM = bgmConfig?.GetBGMForSceneAndState(
            currentSceneIndex,
            currentState,
            conditionTag) ?? defaultBGM;

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

    /// <summary>
    /// 清除当前条件标签
    /// </summary>
    public void ClearConditionTag()
    {
        UpdateBGM(null);
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;

        var source = GetAvailableSFXSource();
        source.PlayOneShot(clip);
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
