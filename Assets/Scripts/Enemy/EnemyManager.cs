using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : SingletonMono<EnemyManager>//TODO:敌人生成器也在这
{
    private List<EnemyCore> activeEnemies = new List<EnemyCore>();

    protected override void Init()
    {
        EnemyEvent.OnSpawned += OnEnemySpawned;
        EnemyEvent.OnDeath += OnEnemyDeath;
    }

    private void OnEnemySpawned(EnemyCore enemy)
    {
        if (!activeEnemies.Contains(enemy))
        {
            activeEnemies.Add(enemy);
        }
    }

    private void OnEnemyDeath(EnemyCore enemy, DamageSource source)
    {
        activeEnemies.Remove(enemy);
    }

    /// <summary>
    /// 亵渎技能
    /// </summary>
    /// <param name="origin">技能中心点</param>
    /// <param name="radius">作用半径</param>
    public void ChainKill(Vector2 origin, float radius, float chainKillDamage)
    {
        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            var enemy = activeEnemies[i];
            if (enemy == null || enemy.IsDead) continue;

            float distance = Vector2.Distance(origin, enemy.transform.position);
            if (distance <= radius)
            {
                //来源标记为ChainKill
                enemy.TakeDamage(chainKillDamage, DamageSource.ChainKill);
            }
        }

        Debug.Log($"亵渎技能生效，杀死了{activeEnemies.Count}个敌人");
    }

    public int GetActiveEnemyCount()
    {
        return activeEnemies.Count;
    }
}