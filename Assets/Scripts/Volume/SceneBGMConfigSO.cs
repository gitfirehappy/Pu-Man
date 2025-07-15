using UnityEngine;

[CreateAssetMenu(fileName = "SceneBGMConfigSO", menuName = "Audio/Scene BGM Config SO")]
public class SceneBGMConfigSO : ScriptableObject
{
    [System.Serializable]
    public struct StateBGM
    {
        public GameState state;
        public AudioClip[] bgmClips; // 改为数组支持多个音乐
        public string condition; // 条件标识符，如"BossBattle"
    }

    [System.Serializable]
    public struct SceneBGM
    {
        public int sceneBuildIndex;
        public StateBGM[] stateBGMs;
    }

    public SceneBGM[] sceneBGMs;

    public AudioClip GetBGMForSceneAndState(int buildIndex, GameState state, string condition = null)
    {
        foreach (var sceneBGM in sceneBGMs)
        {
            if (sceneBGM.sceneBuildIndex == buildIndex)
            {
                foreach (var stateBGM in sceneBGM.stateBGMs)
                {
                    if (stateBGM.state == state)
                    {
                        // 如果没有条件或条件匹配
                        if (string.IsNullOrEmpty(condition) ||
                            stateBGM.condition == condition)
                        {
                            // 随机选择一个音乐
                            if (stateBGM.bgmClips != null && stateBGM.bgmClips.Length > 0)
                                return stateBGM.bgmClips[Random.Range(0, stateBGM.bgmClips.Length)];
                        }
                    }
                }
                return null;
            }
        }
        return null;
    }
}
