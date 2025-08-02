using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;
using System.Linq;

public class DataManager : SingletonMono<DataManager>
{
    private const string SAVE_FILE_NAME = "PlayerWaveRecords.json"; // 存储在游戏同级目录
    //JSON记录
    private Dictionary<PlayerType, int> records = new Dictionary<PlayerType, int>();
    //SO
    private Dictionary<PlayerType, PlayerAnimationSO> _playerAnimationSODict = new Dictionary<PlayerType, PlayerAnimationSO>();
    private Dictionary<EnemyType, EnemyAnimationSO> _enemyAnimationSODict = new Dictionary<EnemyType, EnemyAnimationSO>();
    private Dictionary<PlayerType, PlayerSO> _playerSODict = new Dictionary<PlayerType, PlayerSO>();
    private Dictionary<BuffID, BuffSO> _buffSODict = new Dictionary<BuffID, BuffSO>();
    private Dictionary<Rarity, List<BuffSO>> _buffsByRarity = new Dictionary<Rarity, List<BuffSO>>();

    // 加载状态
    public bool IsPlayerDataLoaded { get; private set; }
    public bool IsAnimationDataLoaded { get; private set; }
    public bool IsBuffDataLoaded { get; private set; }

    protected override void Init()
    {
        Debug.Log("开始加载记录");
        LoadData();
        Debug.Log("开始加载所有SO");
        LoadAllSOsAsync();
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

    private async void LoadAllSOsAsync()
    {
        try
        {
            // 并行加载三种SO
            var loadPlayerSOsTask = LoadPlayerSOsAsync();
            var loadPlayerAnimationSOsTask = LoadPlayerAnimationSOsAsync();
            var loadEnemyAnimationSOsTask = LoadEnemyAnimationSOsAsync();
            var loadBuffSOsTask = LoadBuffSOsAsync();

            await Task.WhenAll(loadPlayerSOsTask, loadPlayerAnimationSOsTask,
                              loadEnemyAnimationSOsTask, loadBuffSOsTask);

            IsPlayerDataLoaded = true;
            IsAnimationDataLoaded = true;
            IsBuffDataLoaded = true;
            Debug.Log("所有SO加载完成");
        }
        catch (Exception e)
        {
            Debug.LogError($"加载SO失败: {e.Message}");
        }
    }

    #region 加载SO方法

    private async Task LoadPlayerSOsAsync()
    {
        var loadHandle = Addressables.LoadAssetsAsync<PlayerSO>("PlayerSO", null);
        await loadHandle.Task;

        if (loadHandle.Status == AsyncOperationStatus.Succeeded)
        {
            foreach (var so in loadHandle.Result)
            {
                _playerSODict[so.playerType] = so;
            }
            Debug.Log($"成功加载{_playerSODict.Count}个PlayerSO");
        }
        else
        {
            Debug.LogError("加载PlayerSO失败");
        }
    }

    private async Task LoadPlayerAnimationSOsAsync()
    {
        var loadHandle = Addressables.LoadAssetsAsync<PlayerAnimationSO>("PlayerAnimationSO", null);
        await loadHandle.Task;

        if (loadHandle.Status == AsyncOperationStatus.Succeeded)
        {
            foreach (var so in loadHandle.Result)
            {
                _playerAnimationSODict[so.playerType] = so;
            }
            Debug.Log($"成功加载{_playerAnimationSODict.Count}个PlayerAnimationSO");
        }
        else
        {
            Debug.LogError("加载PlayerAnimationSO失败");
        }
    }

    private async Task LoadEnemyAnimationSOsAsync()
    {
        var loadHandle = Addressables.LoadAssetsAsync<EnemyAnimationSO>("EnemyAnimationSO", null);
        await loadHandle.Task;

        if (loadHandle.Status == AsyncOperationStatus.Succeeded)
        {
            foreach (var so in loadHandle.Result)
            {
                _enemyAnimationSODict[so.enemyType] = so;
            }
            Debug.Log($"成功加载{_enemyAnimationSODict.Count}个EnemyAnimationSO");
        }
        else
        {
            Debug.LogError("加载EnemyAnimationSO失败");
        }
    }

    private async Task LoadBuffSOsAsync()
    {
        var loadHandle = Addressables.LoadAssetsAsync<BuffSO>("BuffSO", null);
        await loadHandle.Task;

        if (loadHandle.Status == AsyncOperationStatus.Succeeded)
        {
            foreach (var so in loadHandle.Result)
            {
                _buffSODict[so.buffID] = so;

                // 按稀有度分类
                if (!_buffsByRarity.ContainsKey(so.rarity))
                {
                    _buffsByRarity[so.rarity] = new List<BuffSO>();
                }
                _buffsByRarity[so.rarity].Add(so);
            }
            Debug.Log($"成功加载{_buffSODict.Count}个BuffSO");
        }
        else
        {
            Debug.LogError("加载BuffSO失败");
        }
    }

    #endregion

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
   
    #region 外部调用接口
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

    #endregion

    #region 获取SO公共方法
    public PlayerSO GetPlayerSO(PlayerType playerType)
    {
        return _playerSODict.TryGetValue(playerType, out var so) ? so : null;
    }

    public PlayerAnimationSO GetPlayerAnimationSO(PlayerType playerType)
    {
        return _playerAnimationSODict.TryGetValue(playerType, out var so) ? so : null;
    }

    public EnemyAnimationSO GetEnemyAnimationSO(EnemyType enemyType)
    {
        return _enemyAnimationSODict.TryGetValue(enemyType, out var so) ? so : null;
    }

    public List<PlayerSO> GetAllPlayerSOs()
    {
        return _playerSODict.Values.ToList();
    }

    public BuffSO GetBuffSO(BuffID buffID)
    {
        return _buffSODict.TryGetValue(buffID, out var so) ? so : null;
    }

    public List<BuffSO> GetBuffsByRarity(Rarity rarity)
    {
        return _buffsByRarity.TryGetValue(rarity, out var buffs) ? buffs : new List<BuffSO>();
    }

    public List<BuffSO> GetAllBuffSOs()
    {
        return _buffSODict.Values.ToList();
    }

    #endregion

    [System.Serializable]
    private class RecordWrapper
    {
        public List<CharacterRecord> records = new List<CharacterRecord>();
    }
}