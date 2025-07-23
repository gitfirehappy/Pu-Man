using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneBGMConfigSO", menuName = "Audio/Scene BGM Config SO")]
public class SceneBGMConfigSO : ScriptableObject
{
    [System.Serializable]
    public struct StateBGM
    {
        public GameState state;
        public AudioClip defaultBGM; // 默认背景音乐
        public TaggedBGM[] taggedBGMs; // 带标签的BGM配置
    }

    [System.Serializable]
    public struct TaggedBGM
    {
        public string tag; // 音乐标签（如"BossBattle"）
        public AudioClip bgmClip; // 对应的音乐
    }

    [System.Serializable]
    public struct SceneBGM
    {
        public int sceneBuildIndex;
        public StateBGM[] stateBGMs;
    }

    public SceneBGM[] sceneBGMs;

    public AudioClip GetBGMForSceneAndState(int buildIndex, GameState state, string conditionTag)
    {
        // 查找匹配的场景配置
        var sceneConfig = sceneBGMs.FirstOrDefault(s => s.sceneBuildIndex == buildIndex);
        if (sceneConfig.Equals(default(SceneBGM))) return null;

        // 查找匹配的状态配置
        var stateConfig = sceneConfig.stateBGMs.FirstOrDefault(s => s.state == state);
        if (stateConfig.Equals(default(StateBGM))) return null;

        // 优先使用标签匹配的音乐
        if (!string.IsNullOrEmpty(conditionTag))
        {
            var taggedBGM = stateConfig.taggedBGMs.FirstOrDefault(t => t.tag == conditionTag);
            if (taggedBGM.bgmClip != null) return taggedBGM.bgmClip;
        }

        // 返回默认BGM
        return stateConfig.defaultBGM;
    }

}
