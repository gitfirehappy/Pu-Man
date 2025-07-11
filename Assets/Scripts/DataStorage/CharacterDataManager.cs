using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CharacterDataManager : SingletonMono<CharacterDataManager>
{
    private const string SAVE_FILE_NAME = "PlayerWaveRecords.json"; // 存储在游戏同级目录
    private Dictionary<PlayerType, int> records = new Dictionary<PlayerType, int>();

    protected override void Init()
    {
        LoadData();
    }

    private string GetSavePath()
    {
        // 在编辑器模式下使用Assets同级目录，打包后使用应用程序同级目录
#if UNITY_EDITOR
        return Path.Combine(Directory.GetParent(Application.dataPath).FullName, SAVE_FILE_NAME);
#else
        return Path.Combine(Application.dataPath, "../" + SAVE_FILE_NAME);
#endif
    }

    private void LoadData()
    {
        records.Clear();
        string filePath = GetSavePath();

        if (File.Exists(filePath))
        {
            try
            {
                string json = File.ReadAllText(filePath);
                var wrapper = JsonUtility.FromJson<RecordWrapper>(json);
                foreach (var record in wrapper.records)
                {
                    records[record.playerType] = record.highestWave;
                }
                Debug.Log($"成功加载存档文件: {filePath}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"加载存档失败: {e.Message}");
            }
        }
        else
        {
            Debug.Log("未找到存档文件，将创建新存档");
        }
    }

    private void SaveData()
    {
        var wrapper = new RecordWrapper();
        foreach (var kvp in records)
        {
            wrapper.records.Add(new CharacterRecord
            {
                playerType = kvp.Key,
                highestWave = kvp.Value
            });
        }

        string filePath = GetSavePath();
        try
        {
            string json = JsonUtility.ToJson(wrapper, true); // 使用格式化JSON以便阅读
            File.WriteAllText(filePath, json);
            Debug.Log($"存档已保存到: {filePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"保存存档失败: {e.Message}");
        }
    }

    /// <summary>
    /// 更新记录
    /// </summary>
    public void UpdateRecord(PlayerType type, int wave)
    {
        if (!records.ContainsKey(type) || wave > records[type])
        {
            records[type] = wave;
            SaveData();
        }
    }

    public int GetHighestWave(PlayerType type)
    {
        return records.TryGetValue(type, out int wave) ? wave : 0;
    }

    /// <summary>
    /// 重置记录
    /// </summary>
    public void ResetAllRecords()
    {
        records.Clear();
        SaveData(); // 保存空记录
        Debug.Log("已重置所有角色记录");
    }

    [System.Serializable]
    private class RecordWrapper
    {
        public List<CharacterRecord> records = new List<CharacterRecord>();
    }
}